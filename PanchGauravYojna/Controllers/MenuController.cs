using BL.Common;
using BL.Log;
using BL.ManageMaster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MO.Common;
using MO.MenuMaster;
using MO.ProfileUser;
using PanchGauravYojna.Helpers;
using System.Net;
using System.Security.Claims;

namespace PanchGauravYojna.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenu _iMenu;
        private readonly IInputValidator _iInputValidator;
        private readonly IPermission _iPermission;
        private readonly ILog _iLog;
        public MenuController(IMenu iMenu,IInputValidator iInputValidator, IPermission iPermission,ILog iLog)
        {
            _iMenu = iMenu;
            _iInputValidator = iInputValidator;
            _iPermission = iPermission;
            _iLog  = iLog;
        }
        #region SubMenu and Header Menu
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> Menu(string? Guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            #region Permission            
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            var model = Guid != null
                ? await _iMenu.GetMenuDetailByGuid(Guid)
                : new MenuMO();

            model.BtnSave = Guid != null ? "Update" : "Save";
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> saveMenu(MenuMO model)
        {
            try
            {
                if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                {
                    await HttpContext.SignOutAsync("Cookies");
                    return RedirectToAction("Login", "Account");
                }
                string url = "/menu/menu";
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
                if (!permissionResult.status)
                {
                    return Json(permissionResult);
                }

                // --- Main Save Operation ---
                var result = await _iMenu.saveMenu(model);

                // --- Log Success / Warning ---
                var logType = result.status ? EnumAlert.Success : EnumAlert.Warning;
                string IP = CommonHelper.GetClientIp(HttpContext);

                await _iLog.InsertLog(userId, groupId, logType,
                    $"{IP} : {result.message} | Guid = {model.Guid}", 1);

                return Json(result);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Menu",
                    "SaveMenu",                    
                    ex
                );
                // Return safe JSON response to frontend
                return Json(new result
                {
                    status = false,
                    message = "An unexpected error occurred while saving the menu. Please try again later."
                });
            }
        }


        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> SubMenu(string? Guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            //if (permission == null || !permission.CanList)
            //    return Forbid(); // no list permission

            ViewBag.Permission = permission ?? new MenuPermissionMO();


            var model = Guid != null
                ? await _iMenu.GetSubMenuDetailByGuid(Guid)
                : new MenuMO();

            model.BtnSave = Guid != null ? "Update" : "Save";

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> saveSubMenu(MenuMO model)
        {
            try
            {
                if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                {
                    await HttpContext.SignOutAsync("Cookies");
                    return RedirectToAction("Login", "Account");
                }
                string url = "/menu/menu";
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
                if (!permissionResult.status)
                {
                    return Json(permissionResult);
                }

                // --- Input Validation ---
                if (_iInputValidator.ContainsScriptTags(model.Name)
                    //|| _iInputValidator.ContainsScriptTags(model.ActionName)
                    //|| _iInputValidator.ContainsScriptTags(model.Controller)
                    || _iInputValidator.ContainsScriptTags(model.MangalName)
                    || _iInputValidator.ContainsScriptTags(model.OrderNumber.ToString())
                    )
                {
                    return Json(new result
                    {
                        status = false,
                        message = "Invalid input detected. Scripts are not allowed."
                    });
                }

                // --- Main Save Operation ---
                var result = await _iMenu.saveSubMenu(model);

                // --- Log Success / Warning ---
                var logType = result.status ? EnumAlert.Success : EnumAlert.Warning;
                string IP = CommonHelper.GetClientIp(HttpContext);

                if (model.Guid == null)
                {
                    await _iLog.InsertLog(userId, groupId, logType,
                        $"{IP} : {result.message}", 1);
                }
                else
                {
                    await _iLog.InsertLog(userId, groupId, logType,
                        $"{IP} : EditSubMenu: {result.message} | Guid = {model.Guid}", 1);
                }

                return Json(result);
            }
            catch (Exception ex)
            {                
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Menu",
                    "SaveMenu",
                    ex
                );

                return Json(new result
                {
                    status = false,
                    message = "An unexpected error occurred while saving submenu. Please try again later."
                });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("Menu/MenudataList")]
        public async Task<IActionResult> MenudataList(CommonFilter _filter)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }           

            // Parse and validate integer parameters
            if (!int.TryParse(HttpContext.Request.Form["draw"].FirstOrDefault(), out int draw))
            {
                draw = 0; // Set a default value if parsing fails
            }

            if (!int.TryParse(Request.Form["start"].FirstOrDefault(), out int StartIndex) || StartIndex < 0)
            {
                StartIndex = 0; // Default to 0 if invalid
            }

            if (!int.TryParse(Request.Form["length"].FirstOrDefault(), out int PageSize) || PageSize <= 0)
            {
                PageSize = 10; // Default to 10 if invalid
            }
            // Retrieve the value from the request
            var isSearchable = Request.Form["column[0][searchable]"].FirstOrDefault();

            // Validate the value to ensure it's either "true" or "false"
            if (isSearchable != "true" && isSearchable != "false")
            {
                // Handle invalid value (e.g., default to "false" or reject the request)
                isSearchable = "false"; // Default value
            }

            // Now you can safely use the isSearchable variable
            bool isColumnSearchable = Convert.ToBoolean(isSearchable);



            draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault());
            StartIndex = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            PageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault());



            string SortCol = Convert.ToString(Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault());
            // allow only letters, numbers, and underscore (no SQL injection!)
            if (!string.IsNullOrEmpty(SortCol) &&
                !System.Text.RegularExpressions.Regex.IsMatch(SortCol, @"^[a-zA-Z0-9_]+$"))
            {
                SortCol = "DefaultColumn"; // fallback
            }

            string SortDir = Convert.ToString(Request.Form["order[0][dir]"].FirstOrDefault());
            // allow only "asc" or "desc"
            if (SortDir != "asc" && SortDir != "desc")
                SortDir = "asc"; // default

            string SearchField = Convert.ToString(Request.Form["search[value]"].FirstOrDefault());
            // Manual regex validation
            if (!string.IsNullOrEmpty(SearchField) &&
                !System.Text.RegularExpressions.Regex.IsMatch(SearchField, @"^[a-zA-Z0-9\s]+$"))
            {
                return BadRequest(new result
                {
                    status = false,
                    message = "Invalid characters in input."
                });
            }
            _filter.groupId = groupId.ToString();
            _filter.url = "/menu/menu";
            return Json(await _iMenu.MenudataList(draw, SearchField, StartIndex, PageSize, SortCol, SortDir, 1, _filter));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("Menu/SubMenudataList")]
        public async Task<IActionResult> SubMenudataList(CommonFilter _filter)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            // Parse and validate integer parameters
            if (!int.TryParse(HttpContext.Request.Form["draw"].FirstOrDefault(), out int draw))
            {
                draw = 0; // Set a default value if parsing fails
            }

            if (!int.TryParse(Request.Form["start"].FirstOrDefault(), out int StartIndex) || StartIndex < 0)
            {
                StartIndex = 0; // Default to 0 if invalid
            }

            if (!int.TryParse(Request.Form["length"].FirstOrDefault(), out int PageSize) || PageSize <= 0)
            {
                PageSize = 10; // Default to 10 if invalid
            }
            // Retrieve the value from the request
            var isSearchable = Request.Form["column[0][searchable]"].FirstOrDefault();

            // Validate the value to ensure it's either "true" or "false"
            if (isSearchable != "true" && isSearchable != "false")
            {
                // Handle invalid value (e.g., default to "false" or reject the request)
                isSearchable = "false"; // Default value
            }

            // Now you can safely use the isSearchable variable
            bool isColumnSearchable = Convert.ToBoolean(isSearchable);



            draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault());
            StartIndex = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            PageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault());



            string SortCol = Convert.ToString(Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault());
            // allow only letters, numbers, and underscore (no SQL injection!)
            if (!string.IsNullOrEmpty(SortCol) &&
                !System.Text.RegularExpressions.Regex.IsMatch(SortCol, @"^[a-zA-Z0-9_]+$"))
            {
                SortCol = "DefaultColumn"; // fallback
            }

            string SortDir = Convert.ToString(Request.Form["order[0][dir]"].FirstOrDefault());
            // allow only "asc" or "desc"
            if (SortDir != "asc" && SortDir != "desc")
                SortDir = "asc"; // default

            string SearchField = Convert.ToString(Request.Form["search[value]"].FirstOrDefault());
            // Manual regex validation
            if (!string.IsNullOrEmpty(SearchField) &&
                !System.Text.RegularExpressions.Regex.IsMatch(SearchField, @"^[a-zA-Z0-9\s]+$"))
            {
                return BadRequest(new result
                {
                    status = false,
                    message = "Invalid characters in input."
                });
            }
            _filter.groupId = groupId.ToString();
            _filter.url = "/menu/submenu";
            return Json(await _iMenu.SubMenudataList(draw, SearchField, StartIndex, PageSize, SortCol, SortDir, 1, _filter));
        }
        
        public async Task<IActionResult> IsActiveMenu(int MenuId)
        {
            try
            {
                if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                {
                    await HttpContext.SignOutAsync("Cookies");
                    return RedirectToAction("Login", "Account");
                }
                string url = "/menu/menu";
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_CanActive(MenuId.ToString(), permission);
                if (!permissionResult.status)
                {
                    return BadRequest(permissionResult);
                }

                // --- Operation ---
                var result = await _iMenu.IsActiveMenu(MenuId);

                // --- Log Success / Warning ---
                var logType = result.status ? EnumAlert.Success : EnumAlert.Warning;

                await _iLog.InsertLog(
                    userId,
                    groupId,
                    logType,
                    $"IsActiveMenu: {result.message} | MenuId = {MenuId}",
                    1
                );

                return Json(result);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Menu",
                    "IsActiveMenu",
                    ex
                );

                return Json(new
                {
                    status = false,
                    message = "Something went wrong while updating status."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenu(string guid)
        {
            try
            {
                if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                {
                    await HttpContext.SignOutAsync("Cookies");
                    return RedirectToAction("Login", "Account");
                }

                // --- Main Operation ---
                var result = await _iMenu.DeleteMenu(guid);

                // --- Insert Log (Success / Warning) ---
                var logType = result.status ? EnumAlert.Success : EnumAlert.Warning;

                await _iLog.InsertLog(
                    userId,
                    groupId,
                    logType,
                    $"DeleteMenu: {result.message}. Guid = {guid}",
                    1
                );

                return Json(result);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Menu",
                    "IsActiveMenu",
                    ex
                );
                return Json(new
                {
                    status = false,
                    message = "Something went wrong while deleting menu."
                });
            }
        }

        public async Task<IActionResult> BindDropDown_HeaderMenu()
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            return Json(await _iMenu.BindDropDown_HeaderMenu());
        }
        #endregion
    }
}
