using BL.Common;
using BL.Department;
using BL.GroupMaster;
using BL.ManageMaster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MO.Common;
using MO.GroupMaster;
using MO.Master;
using MO.MenuMaster;
using PanchGauravYojna.Helpers;
using System;
using System.Security.Claims;

namespace PanchGauravYojna.Controllers
{
    public class GroupMasterController : Controller
    {
        private readonly IGroupMaster _iGroupMaster;
        private readonly IMenu _iMenu;
        private readonly IInputValidator _iInputValidator;
        private readonly IPermission _iPermission;
        public GroupMasterController(IGroupMaster iIGroupMaster, IInputValidator iInputValidator, IMenu iMenu, IPermission iPermission)
        {
            _iGroupMaster = iIGroupMaster;
            _iInputValidator = iInputValidator;
            _iMenu = iMenu;
            _iPermission = iPermission;
        }


        #region Group
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> Group(string? Guid)
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
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            GroupMO model;
            var list = await _iGroupMaster.GetAllGroup();
            if (!string.IsNullOrEmpty(Guid))
            {
                // Get single record from list
                //var all = await _iIDepartment.GetAllDepartment();
                model = list.FirstOrDefault(x => x.Guid == Guid) ?? new GroupMO();
                model.BtnSave = "Update";
            }
            else
            {
                model = new GroupMO();
                model.BtnSave = "Save";
            }
            model.list = list;
            return View(model);   // View: Manage.cshtml
        }

        // ================= SAVE / UPDATE =================
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Group(GroupMO model)
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
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion


            var list = await _iGroupMaster.GetAllGroup();
            if (!ModelState.IsValid)
            {
                model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                return View(model);
            }

            model.CreatedBy = Convert.ToString(User.FindFirst("UserId")?.Value); // TODO: replace with user session

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_AddEdit(model.Guid, permission);
            if (!permissionResult.status)
            {
                TempData["ErrorMessage"] = permissionResult.message;
                model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                model.list = list;
                return View(model);
            }            

            var rs = await _iGroupMaster.SaveGroup(model);

            if (rs.status)
            {
                TempData["SuccessMessage"] = rs.message;
                return RedirectToAction("Group");
            }
            else
            {
                TempData["ErrorMessage"] = rs.message;
                model.BtnSave = string.IsNullOrEmpty(model.Guid) ? "Save" : "Update";
                model.list = list;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string guid)
        {
            // --- Claims Expired Check ---
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            #region Permission            
            string url = "/groupmaster/group";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion           

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }

            var result = await _iGroupMaster.ToggleActive(guid);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string guid)
        {
            // --- Claims Expired Check ---
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            #region Permission            
            string url = "/groupmaster/group";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion  

            // --- Permission Check (Reusable) ---
            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
            {
                return Json(permissionResult);
            }

            var result = await _iGroupMaster.Delete(guid);
            return Json(result);
        }
        #endregion
        #region Menu Permission
        [HttpGet]
        public async Task<IActionResult> GroupPermission(string? Guid)
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetGroupPermissions(int GroupId)
        {
            
            var menuList = await _iMenu.GetMenuPermissiondataList(new CommonFilter { option1 = GroupId.ToString() });

            var treeView = BuildTree(menuList, null); // Convert to hierarchical structure
            return Ok(treeView);
        }

        // Recursive function to build the tree
        private List<MenuItem> BuildTree(List<MenuItem> menuItems, int? parentId)
        {
            return menuItems.Where(m => m.ParentId == parentId)
                .Select(m => new MenuItem
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    Name = m.Name,
                    OrderNumber = m.OrderNumber,

                    // permissions add किए गए हैं
                    isChecked = m.isChecked,
                    CanAdd = m.CanAdd,
                    CanList = m.CanList,
                    CanEdit = m.CanEdit,
                    CanDelete = m.CanDelete,
                    CanActiveDeactive = m.CanActiveDeactive,

                    // recursive children load
                    Children = BuildTree(menuItems, m.Id)
                }).ToList();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> saveGroupPermissions(SaveGroupPermissionModel model)
        {            
            return Json(await _iMenu.saveMenuPermissions(model));
        }

        public async Task<IActionResult> BindDropDown_Group()
        {
            return Json(await _iGroupMaster.BindDropDown_Group());
        }
        #endregion
    }
}
