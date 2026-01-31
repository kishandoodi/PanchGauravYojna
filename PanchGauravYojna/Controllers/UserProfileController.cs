using BL.Common;
using BL.GroupMaster;
using BL.ProfileUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MO.Common;
using MO.ProfileUser;
using System;
using System.Security.Claims;

namespace PanchGauravYojna.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly ICommon _iCommon;
        private readonly IProfileUser _iProfileUser;
        private readonly IPermission _iPermission;

        public UserProfileController(ICommon iCommon, IProfileUser iProfile, IPermission iPermission)
        {
            _iCommon = iCommon;
            _iProfileUser = iProfile;
            _iPermission = iPermission;
        }
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> AddUser(string? guid)
        {

            #region Permission
            int groupId = Convert.ToInt32(User.FindFirstValue("GroupId"));
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            if (!string.IsNullOrEmpty(guid))
            {
                if (!permission.CanEdit)
                {
                    TempData["Error"] = "You do not have permission to edit this user.";
                    return RedirectToAction("List");
                }

                var model = await _iProfileUser.GetUserByGuidAsync(guid);
                if (model != null)
                {
                    //model.DistrictList = await _iCommon.DDL_DistrictAsync();
                    var distList = await _iCommon.DDL_DistrictAsync();
                    // Insert "All" option at the top
                    distList.Insert(0, new DistrictListDDL
                    {
                        Id = 0,
                        DistrictName = "All"
                    });
                    model.DistrictList = distList;

                    var deptList = await _iCommon.DDL_DepartmentAsync();
                    // Insert "All" option at the top
                    deptList.Insert(0, new DepartmentListDDL
                    {
                        Id = 0,
                        DepartmentName = "All"
                    });
                    model.DepartmentList = deptList;


                    //model.DepartmentList = await _iCommon.DDL_DepartmentAsync();
                    //model.RoleList = await _iCommon.DDL_RoleAsync();
                    model.GroupList = await _iCommon.DDL_GroupAsync();
                    model.UserLevelList = await _iCommon.DDL_UserLevelAsync();
                    return View(model);
                }
            }
            else
            {
                var distList = await _iCommon.DDL_DistrictAsync();
                // Insert "All" option at the top
                distList.Insert(0, new DistrictListDDL
                {
                    Id = 0,
                    DistrictName = "All"
                });

                var deptList = await _iCommon.DDL_DepartmentAsync();
                // Insert "All" option at the top
                deptList.Insert(0, new DepartmentListDDL
                {
                    Id = 0,
                    DepartmentName = "All"
                });

                var model = new UserMO
                {   
                      DistrictList = distList,                    
                      DepartmentList = deptList,
                      GroupList = await _iCommon.DDL_GroupAsync(),
                      UserLevelList = await _iCommon.DDL_UserLevelAsync()
                };
                return View(model);               
            }
            return View(new UserMO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserMO model)
        {
            #region Permission
            int groupId = Convert.ToInt32(User.FindFirstValue("GroupId"));
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            if (!permission.CanAdd && model.Guid == null)
            {
                TempData["ErrorMessage"] = "You do not have permission to add a new user.";
                return RedirectToAction("List");
            }
            if (!permission.CanEdit && model.Guid !=null)
            {
                TempData["ErrorMessage"] = "You do not have permission to eidt a user.";
                return RedirectToAction("List");
            }
            var result = await _iProfileUser.SaveUserAsync(model);
            if (result.status)
            {
                TempData["SuccessMessage"] = result.message;
                return RedirectToAction("ListUser");
            }
            else
            {
                TempData["ErrorMessage"] = "somethisng wrong";
                return View(model);
            }
            //return Json(result);
        }

        [Authorize(Policy = "PageAccess")]
        public async Task<IActionResult> ListUser()
        {
            #region Permission
            int groupId = Convert.ToInt32(User.FindFirstValue("GroupId"));
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            MenuPermissionMO permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion
            var list = await _iProfileUser.GetUserListAsync();
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(string guid)
        {
            #region Permission
            int groupId = Convert.ToInt32(User.FindFirstValue("GroupId"));
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            if(!permission.CanActiveDeactive)
            {
                result result = new result
                {
                    status = false,
                    message = "You do not have permission to change status a user."
                };
                return Json(result);
            }

            var modifiedBy = User.Identity?.Name ?? "System";
            var res = await _iProfileUser.ActiveDeactiveUserAsync(guid, modifiedBy);  
            return Json(res);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string guid)
        {
            #region Permission
            int groupId = Convert.ToInt32(User.FindFirstValue("GroupId"));
            string url = (HttpContext.Request.Path.Value ?? "").ToLower();

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);
            ViewBag.Permission = permission ?? new MenuPermissionMO();
            #endregion

            if (!permission.CanDelete)
            {
                result res = new result
                {
                    status = false,
                    message = "You do not have permission to delete a user."
                };
                return Json(res);
            }

            var modifiedBy = User.Identity?.Name ?? "System";
            var result = await _iProfileUser.Delete(guid,modifiedBy);
            return Json(result);
        }
    }
}
