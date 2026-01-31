using BL.Common;
using BL.Department;
using BL.FinancialYear;
using BL.GauravMaster;
using BL.Log;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MO.Common;
using MO.Master;
using MO.ProfileUser;
using PanchGauravYojna.Helpers;
using PanchGauravYojna.Models;
using System;
using System.Security.Claims;
using System.Security.Principal;
   
namespace PanchGauravYojna.Controllers
{
    //[Authorize(Roles = "Admin Des,Supervisor,Nodal Officer")]
    public class MasterController : Controller
    {
        private readonly ICommon _iCommon;
        private readonly IDepartment _iIDepartment;
        private readonly IGauravMaster _iIGauravMaster;
        private readonly IPermission _iPermission;
        private readonly ILog _iLog;
        private readonly IGauravDistrict _iGauravDistrict;
        private readonly IFinancialYear _iFinancialYear;

        public MasterController(ICommon iCommon, IDepartment iDepartemnt, IPermission iPermission,IGauravMaster iGauravMaster,ILog iLog,IGauravDistrict iGauravDistrict, IFinancialYear iFinancialYear)
        {
            _iCommon = iCommon;
            _iIDepartment = iDepartemnt;
            _iIGauravMaster = iGauravMaster;
            _iPermission = iPermission;
            _iLog = iLog;
            _iGauravDistrict= iGauravDistrict;
            _iFinancialYear = iFinancialYear;
        }

        
        //[Authorize(Roles = "Admin Des,Supervisor,Nodal Officer")]
        public IActionResult Index()
        {
            return View();
        }

        #region Common
        //[Authorize(Roles = "Admin Des,Supervisor,Nodal Officer")]
        [Route("Master/BindGauravDropDown")]
        public async Task<IActionResult> BindGauravDropDown()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            string userId = User.FindFirst("UserId")?.Value;
            return Json(await _iCommon.BindGauravDropDown(userId));
        }
        #endregion
        #region Common
        //[Authorize(Roles = "Admin Des")]
        [Route("Master/BindDistrictDropDown")]
        public async Task<IActionResult> BindDistrictDropDown()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            string userId = User.FindFirst("UserId")?.Value;
            return Json(await _iCommon.BindDistrictDropDown(userId));
        }
        #endregion

        #region Department Master   
        // ================= ADD / EDIT PAGE =================
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> Department(string? Guid)
        {
            // --- Claims Expired Check ---
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            #region Permission            
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            try
            {
                ViewBag.Permission = permission ?? new MenuPermissionMO();
            }
            catch(Exception ex)
            {

            }
            #endregion

            DepartmentMO model;
            var list = await _iIDepartment.GetAllDepartment();
            if (!string.IsNullOrEmpty(Guid))
            {
                // Get single record from list
                var all = await _iIDepartment.GetAllDepartment();
                model = all.FirstOrDefault(x => x.Guid == Guid) ?? new DepartmentMO();
                model.BtnSave = "Update";
            }
            else
            {
                model = new DepartmentMO();
                model.BtnSave = "Save";
            }
            model.Department_list = list;
            return View(model);   // View: Manage.cshtml
        }


        // ================= SAVE / UPDATE =================
        [HttpPost]
        public async Task<IActionResult> Department(DepartmentMO model)
        {
             // --- Claims Expired Check ---
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            try
            {
                // ========== Permission ==========
                string url = (HttpContext.Request.Path.Value ?? "").ToLower();
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
                if (!permissionResult.status)
                {
                    TempData["ErrorMessage"] = permissionResult.message;
                    // Reload dropdown/list for page
                    model.Department_list = await _iIDepartment.GetAllDepartment();
                    // Set Save/Update button text
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    return View(model);   // ← Return same view, not JSON
                }

                // ========== Load dropdown list ==========
                var list = await _iIDepartment.GetAllDepartment();

                // ========== Model Validation ==========
                if (!ModelState.IsValid)
                {
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    model.Department_list = list;
                    return View(model);
                }

                model.CreatedBy = Convert.ToInt64(User.FindFirst("UserId")?.Value);

                // ========== Save / Update ==========
                var rs = await _iIDepartment.SaveDepartment(model);
                string IP = CommonHelper.GetClientIp(HttpContext);
                // ========== Log & Response ==========
                if (rs.status)
                {                    
                    await _iLog.InsertLog(userId, groupId,EnumAlert.Success, IP + " : " + rs.message,1);
                    TempData["SuccessMessage"] = rs.message;                   
                    return RedirectToAction("Department");
                }
                else
                {
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Warning, IP + " : " + rs.message,1);
                    TempData["ErrorMessage"] = rs.message;
                    model.Department_list = list;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "Department",
                    ex
                );
                TempData["ErrorMessage"] = "Something went wrong.";
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ToggleActive(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            #region Permission           
            string url = "/master/department";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }

            result toggleResult;
            try
            {
                // 2️⃣ Main operation
                toggleResult = await _iIDepartment.ToggleActive(guid);
                string IP = CommonHelper.GetClientIp(HttpContext);
                // 3️⃣ Log based on success/warning
                EnumAlert logType = toggleResult.status ? EnumAlert.Success : EnumAlert.Warning;
                await _iLog.InsertLog(userId, groupId, logType, IP + " : " + toggleResult.message, 1);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "ToggleActive",
                    ex
                );               

                // 5️⃣ Send safe response to UI
                toggleResult = new result
                {
                    status = false,
                    message = "Something went wrong. Please try again later."
                };
            }

            return Json(toggleResult);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }  
            try
            {
                #region Permission           
                string url = "/master/department";

                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();
                #endregion

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
                if (!permissionResult.status)
                {
                    return Json(permissionResult);
                }

                var result = await _iIDepartment.Delete(guid);
                string IP = CommonHelper.GetClientIp(HttpContext);
                if (result.status)
                {
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Success, IP + " : " + result.message, 1);
                }
                else
                {
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Warning, IP + " : " + result.message, 1);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "Delete",
                    ex
                );               

                return Json(new
                {
                    status = false,
                    message = "Something went wrong while deleting."
                });
            }
        }

        #endregion

        #region Gaurav With Department Master
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> Gaurav(string? Guid)
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

            GauravMasterMO model;
            var list = await _iIGauravMaster.GetAllGaurav();
            if (!string.IsNullOrEmpty(Guid))
            {
                // Get single record from list
                var all = await _iIGauravMaster.GetAllGaurav();
                model = all.FirstOrDefault(x => x.Guid == Guid) ?? new GauravMasterMO();
                model.BtnSave = "Update";
            }
            else
            {
                model = new GauravMasterMO();
                model.BtnSave = "Save";
            }
            model._list = list;
            return View(model);   // View: Manage.cshtml
        }

        // ================= SAVE / UPDATE =================
        [HttpPost]
        public async Task<IActionResult> Gaurav(GauravMasterMO model)
        {
            try
            {
                // --- Claims & User Validation ---
                if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                {
                    await HttpContext.SignOutAsync("Cookies");
                    return RedirectToAction("Login", "Account");
                }


                // ========== Permission ==========
                string url = (HttpContext.Request.Path.Value ?? "").ToLower();
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
                if (!permissionResult.status)
                {
                    TempData["ErrorMessage"] = permissionResult.message;
                    // Reload dropdown/list for page
                    model._list = await _iIGauravMaster.GetAllGaurav();
                    // Set Save/Update button text
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    return View(model);   // ← Return same view, not JSON
                }

                var list = await _iIGauravMaster.GetAllGaurav();

                // --- Model Validation ---
                if (!ModelState.IsValid)
                {
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    model._list = list;
                    return View(model);
                }

                // --- Create By ---
                model.CreatedBy = Convert.ToInt64(User.FindFirst("UserId")?.Value);

                // --- Save Operation ---
                var rs = await _iIGauravMaster.SaveGaurav(model);

                if (rs.status)
                {
                    TempData["SuccessMessage"] = rs.message;
                    return RedirectToAction("Gaurav");
                }
                else
                {
                    TempData["ErrorMessage"] = rs.message;
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    model._list = list;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "Gaurav-Post",
                    ex
                );
                TempData["ErrorMessage"] = "Something went wrong. Please try again later.";
                return RedirectToAction("Gaurav");
            }
        }        

        [HttpPost]
        public async Task<IActionResult> GauravToggleActive(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            #region Permission           
            string url = "/master/gaurav";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }
            result toggleResult;

            try
            {
                // ---------- Main Operation ----------
                toggleResult = await _iIGauravMaster.ToggleActive(guid);

                // ---------- Insert Success/Warning Log ----------
                var logType = toggleResult.status ? EnumAlert.Success : EnumAlert.Warning;
                string logMessage = "GauravMaster - ToggleActive : " + toggleResult.message;
                string IP = CommonHelper.GetClientIp(HttpContext);
                await _iLog.InsertLog(userId, groupId, logType, IP + " : " + logMessage, 1);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "GauravMaster - ToggleActive",
                    ex
                );               

                // ---------- Return Error Response ----------
                toggleResult = new result
                {
                    status = false,
                    message = "Something went wrong. Please try again later."
                };
            }
            return Json(toggleResult);
        }

        [HttpPost]
        public async Task<IActionResult> GauravDelete(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            #region Permission           
            string url = "/master/gaurav";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }
            result deleteResult;
            try
            {
                // ---------- Main Delete Operation ----------
                deleteResult = await _iIGauravMaster.Delete(guid);

                // ---------- Log Success / Warning ----------
                var logType = deleteResult.status ? EnumAlert.Success : EnumAlert.Warning;
                string msg = "GauravMaster - Delete : " + deleteResult.message;
                string IP = CommonHelper.GetClientIp(HttpContext);
                await _iLog.InsertLog(userId, groupId, logType, IP + " : " + msg, 1);
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "GauravMaster - Delete",
                    ex
                );

                deleteResult = new result
                {
                    status = false,
                    message = "Something went wrong while deleting. Please try again later."
                };
            }

            return Json(deleteResult);
        }

        #endregion
        #region Gaurav Department
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> GauravDepartment(string? Guid)
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

            GauravDepartmentMO model;
            var list = await _iIGauravMaster.GetAllGauravDepartment();
            if (!string.IsNullOrEmpty(Guid))
            {
                // Get single record from list
                var all = await _iIGauravMaster.GetAllGauravDepartment();
                model = all.FirstOrDefault(x => x.Guid == Guid) ?? new GauravDepartmentMO();
                model.BtnSave = "Update";
            }
            else
            {
                model = new GauravDepartmentMO();
                model.BtnSave = "Save";
            }
            // Dropdowns
            model.GauravList = await _iCommon.DDL_GauravAsync();          
            model.DepartmentList = await _iCommon.DDL_DepartmentAsync();
            model._list = list;
            return View(model);   // View: Manage.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> GauravDepartment(GauravDepartmentMO model)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            try
            {
                // Load dropdowns & list
                var list = await _iIGauravMaster.GetAllGauravDepartment();
                model.GauravList = await _iCommon.DDL_GauravAsync();
                model.DepartmentList = await _iCommon.DDL_DepartmentAsync();
                model._list = list;
                if (!ModelState.IsValid)
                {
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    return View(model);
                }

                // ========== Permission ==========
                string url = (HttpContext.Request.Path.Value ?? "").ToLower();
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
                if (!permissionResult.status)
                {
                    TempData["ErrorMessage"] = permissionResult.message;                    
                    // Set Save/Update button text
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    return View(model);   // ← Return same view, not JSON
                }
                model.CreatedBy = Convert.ToInt64(userId);

                // ---------- Save Operation ----------
                var rs = await _iIGauravMaster.SaveGauravDepartment(model);

                if (rs.status)
                {
                    string IP = CommonHelper.GetClientIp(HttpContext);
                    // Log success
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Success, IP + " : " +
                         rs.message, 1);

                    TempData["SuccessMessage"] = rs.message;
                    return RedirectToAction("GauravDepartment");
                }
                else
                {
                    // Log warning
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Warning,
                        "GauravDepartment - Save : " + rs.message, 1);

                    TempData["ErrorMessage"] = rs.message;
                    model._list = list;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "GauravDepartment - Save",
                    ex
                );               

                TempData["ErrorMessage"] = "Something went wrong. Please try again later.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GauravDepartmentToggleActive(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            #region Permission           
            string url = "/master/gauravdepartment";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
            {
                return BadRequest(permissionResult);
            }

            result toggleResult;

            try
            {
                // Perform the toggle operation
                toggleResult = await _iIGauravMaster.GauravDepartmentToggleActive(guid);

                // Log success or warning
                var logType = toggleResult.status ? EnumAlert.Success : EnumAlert.Warning;
                string IP = CommonHelper.GetClientIp(HttpContext);
                await _iLog.InsertLog(
                    userId,
                    groupId,
                    logType,
                    IP + " : "  + toggleResult.message,
                    1
                );
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "GauravDepartment-ToggleActive",
                    ex
                );                

                toggleResult = new result
                {
                    status = false,
                    message = "Something went wrong. Please try again later."
                };
            }

            return Json(toggleResult);
        }

        [HttpPost]
        public async Task<IActionResult> GauravDepartmentDelete(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            #region Permission           
            string url = "/master/gaurav";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }
            result deleteResult;

            try
            {
                // Perform delete operation
                deleteResult = await _iIGauravMaster.GauravDepartmentDelete(guid);

                // Log success or warning
                var logType = deleteResult.status ? EnumAlert.Success : EnumAlert.Warning;
                string IP = CommonHelper.GetClientIp(HttpContext);
                await _iLog.InsertLog(
                    userId,
                    groupId,
                    logType,
                    IP + " : " + deleteResult.message,
                    1
                );
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "Master",
                    "GauravDepartment-Delete",
                    ex
                );               

                deleteResult = new result
                {
                    status = false,
                    message = "Something went wrong. Please try again later."
                };
            }

            return Json(deleteResult);
        }
        #endregion


        #region GauravDistrict
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> GauravDistrictItem(string? Guid)
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
            
            GauravDistrictMO model= new GauravDistrictMO();
            model.FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            var list = await _iGauravDistrict.GetList(Convert.ToInt32(model.FyId));

            if (!string.IsNullOrEmpty(Guid))
            {
                model = list.FirstOrDefault(x => x.Guid == Guid) ?? new GauravDistrictMO();

                model.BtnSave = "Update";
            }
            else
            {
                model = new GauravDistrictMO();
                model.BtnSave = "Save";
            }
            model.DistrictList = await _iCommon.DDL_DistrictAsync();
            model.GauravList = await _iCommon.DDL_GauravAsync();
            model.list = list;
            return View(model);   // View: Manage.cshtml (same pattern)
        }
        // ================= SAVE / UPDATE =================
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GauravDistrictItem(GauravDistrictMO model)
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
            model.FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            var list = await _iGauravDistrict.GetList(Convert.ToInt32(model.FyId));
            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
            if (!permissionResult.status)
            {
                TempData["ErrorMessage"] = permissionResult.message;
                model.list = list;
                model.DistrictList = await _iCommon.DDL_DistrictAsync();
                model.GauravList = await _iCommon.DDL_GauravAsync();
                // Set Save/Update button text
                model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                return View(model);   // ← Return same view, not JSON
            }

            if (!ModelState.IsValid)
            {
                model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                model.list = list;
                model.DistrictList = await _iCommon.DDL_DistrictAsync();
                model.GauravList = await _iCommon.DDL_GauravAsync();
                return View(model);
            }

            model.CreatedBy = Convert.ToString(User.FindFirst("UserId")?.Value);


            var rs = await _iGauravDistrict.Save(model);
            if (rs.status)
            {
                TempData["SuccessMessage"] = rs.message;
                return RedirectToAction("GauravDistrictItem");
            }
            else
            {
                TempData["ErrorMessage"] = rs.message;
                model.list = list;
                model.DistrictList = await _iCommon.DDL_DistrictAsync();
                model.GauravList = await _iCommon.DDL_GauravAsync();
                return View(model);
            }
        }

        #region TOGGLE ACTIVE
        [HttpPost]
        public async Task<IActionResult> GauravDistToggleActive(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            #region Permission           
            string url = "/master/gauravdistrictitem";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
            {
                return BadRequest(permissionResult);
            }
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            var result = await _iGauravDistrict.ToggleActive(guid, FyId);
            return Json(result);
        }
        #endregion

        #region DELETE
        [HttpPost]
        public async Task<IActionResult> GauravDistDelete(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            #region Permission            
            string url = "/master/gauravdistrictitem";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            var result = await _iGauravDistrict.Delete(guid, FyId);
            return Json(result);
        }
        #endregion
        #endregion

        #region ChangeYear
        public async Task<IActionResult> ChangeFinancialyear()
        {
            ChangeSession modal = new ChangeSession();
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

            // ✅ await is MUST
            modal.FinancialYears = await _iCommon.DDL_FinancialYearAsync();            

            return View(modal);
            
        }
        [HttpPost]
        public async Task<IActionResult> ChangeFinancialyear(ChangeSession modal)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }
            FinancialYearListMO modalpass = new FinancialYearListMO();
            modalpass.FinancialYear_Id = modal.Id;
            List<FinancialYearListMO> _List = await _iCommon.FinancialYearList(modalpass);

            // 🔹 Remove old claims safely
            var claimsToRemove = identity.Claims
                .Where(c => c.Type == "FinancialYear"
                         || c.Type == "Financialtext"
                         )   // second claim
                .ToList();

            foreach (var claim in claimsToRemove)
            {
                identity.RemoveClaim(claim);
            }

            // 🔹 Add new claims
            identity.AddClaim(new Claim("FinancialYear", modal.Id.ToString()));
            identity.AddClaim(new Claim("Financialtext", _List.FirstOrDefault().FinancialYear)); // assuming modal.Text exists            

            // re-issue authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
            TempData["SuccessMessage"] = "Financial Year Session Chnaged Successfully";
            return RedirectToAction("ChangeFinancialyear", "Master"); // or Ok()
        }
        #endregion

        #region Financial Year Master
        //[Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> FinancialYear(string? guid)
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

            FinancialYearCM model;
            var list = await _iFinancialYear.GetAllFinancialYear();

            if (!string.IsNullOrEmpty(guid))
            {
                model = list.FirstOrDefault(x => x.Guid == guid) ?? new FinancialYearCM();
                model.BtnSave = "Update";
            }
            else
            {
                model = new FinancialYearCM { BtnSave = "Save" };
            }

            model.FinancialYear_List = list;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FinancialYear(FinancialYearCM model)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                string url = (HttpContext.Request.Path.Value ?? "").ToLower();
                var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
                ViewBag.Permission = permission ?? new MenuPermissionMO();

                var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
                if (!permissionResult.status)
                {
                    TempData["ErrorMessage"] = permissionResult.message;
                    model.FinancialYear_List = await _iFinancialYear.GetAllFinancialYear();
                    model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    model.FinancialYear_List = await _iFinancialYear.GetAllFinancialYear();
                    return View(model);
                }

                model.CreatedBy = Convert.ToInt64(User.FindFirst("UserId")?.Value);

                var rs = await _iFinancialYear.SaveFinancialYear(model);
                string IP = CommonHelper.GetClientIp(HttpContext);

                if (rs.status)
                {
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Success, IP + " : " + rs.message, 1);
                    TempData["SuccessMessage"] = rs.message;
                    return RedirectToAction("FinancialYear");
                }
                else
                {
                    await _iLog.InsertLog(userId, groupId, EnumAlert.Warning, IP + " : " + rs.message, 1);
                    TempData["ErrorMessage"] = rs.message;
                    model.FinancialYear_List = await _iFinancialYear.GetAllFinancialYear();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                await _iLog.ErrorInsertLog("Master", "FinancialYear", ex);
                TempData["ErrorMessage"] = "Something went wrong.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive_FinancialYear(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/master/financialyear";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);

            var result = await _iFinancialYear.ToggleActive(guid);
            return Json(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> IsCurrent_FinancialYear(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/master/financialyear";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);

            var result = await _iFinancialYear.IsCurrent(guid);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> Delete_FinancialYear(string guid)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/master/financialyear";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);

            return Json(await _iFinancialYear.Delete(guid));
        }
        #endregion

    }
}
