using BL.Common;
using BL.Log;
using BL.WebsiteMaster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.Common;
using MO.WebsiteMaster;
using PanchGauravYojna.Helpers;
using System;
using System.Threading.Tasks;

namespace PanchGauravYojna.Controllers
{
    public class WebsiteMasterController : Controller       
    {
        #region properties
        private readonly IWebHostEnvironment _env;
        private readonly ISliderVM _sliderService;
        private readonly IPermission _iPermission;
        private readonly ILog _iLog;
        #endregion
        #region Constructor
        public WebsiteMasterController(IWebHostEnvironment env,ISliderVM sliderService,IPermission iPermission,ILog iLog)
        {
            _env = env;
            _sliderService = sliderService;
            _iPermission = iPermission;
            _iLog = iLog;
        }
        #endregion
        #region slider image get,save & update
        [Authorize(Policy = "PageAccess")]
        [HttpGet]
        public async Task<IActionResult> SliderVM()
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
            catch (Exception ex)
            {

            }
            #endregion

            var model = new SliderCreateVM();
            model.SliderList = await _sliderService.GetAllSliderImage();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SliderVM(SliderCreateVM model)
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

                if ((model.Id == null || model.Id == 0) &&
                (model.ImageFile == null || model.ImageFile.Length == 0))
                {
                    ModelState.AddModelError("ImageFile", "Image is required");
                }

                if (!ModelState.IsValid)
                {
                    model.SliderList = await _sliderService.GetAllSliderImage();
                    return View(model);
                }
                string imageBase64 = model.ExistingImage;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await model.ImageFile.CopyToAsync(ms);

                    imageBase64 = "data:image/png;base64," +
                                  Convert.ToBase64String(ms.ToArray());
                }

                var entity = new mst_Slider
                {
                    Id = model.Id ?? 0,
                    Title = model.Title,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    ImageBase64 = imageBase64
                };
                string? guid = null;
                if(model.Id == null)
                {
                    guid = null;
                }
                else
                {
                    guid = model.Id.ToString();
                }
                    // --- Permission Check (Reusable) ---
                    var permissionResult = CommonHelper.HasPermission_AddEdit(guid, permission);
                if (!permissionResult.status)
                {
                    TempData["ErrorMessage"] = permissionResult.message;
                    model.SliderList = await _sliderService.GetAllSliderImage();
                    return View(model);                    
                }
                if (model.Id > 0)
                    await _sliderService.UpdateSliderAsync(entity);   // 👈 UPDATE
                else
                    await _sliderService.SaveSliderAsync(entity);     // 👈 INSERT
                return RedirectToAction(nameof(SliderVM));
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "WebsiteMaster",
                    "SliderVM",
                    ex
                );
                TempData["ErrorMessage"] = "Something went wrong.";
                return View(model);
            }
        }


        #endregion
        #region delete & Active Deactive slider image row
        [HttpPost]
        public async Task<IActionResult> deleteSliderRow(int rowId)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            //int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/websitemaster/slidervm";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            string? guid = null;
            if (rowId == null)
            {
                guid = null;
            }
            else
            {
                guid = rowId.ToString();
            }

            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);

            return Json(await _sliderService.deleteSliderRow(rowId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSliderStatus([FromBody] ToggleSliderVM model)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/websitemaster/slidervm";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            string? guid = null;
            if (model.Id == null)
            {
                guid = null;
            }
            else
            {
                guid = model.Id.ToString();
            }

            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);            

            var result = await _sliderService.ToggleStatusAsync(model.Id, model.IsActive);
            return Json(result);
        }

        #endregion
        #region Announcement list save & update
        [Authorize(Policy = "PageAccess")]
        public async Task<IActionResult> Announcement()
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
            catch (Exception ex)
            {

            }
            #endregion

            var model = new Announcement();
            model.AnnouncementList = await _sliderService.GetAllAnnouncement();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Announcement(Announcement model)
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

                if ((model.Id == null || model.Id == 0) &&
                (model.ImageFile == null || model.ImageFile.Length == 0))
                {
                    ModelState.AddModelError("ImageFile", "file is required");
                }
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var ext = Path.GetExtension(model.ImageFile.FileName).ToLower();

                    if (ext != ".pdf")
                    {
                        ModelState.AddModelError("ImageFile", "Only PDF allowed");
                    }
                }
                if (!ModelState.IsValid)
                {
                    model.AnnouncementList = await _sliderService.GetAllAnnouncement();
                    return View(model);
                }

                string imageBase64 = model.ExistingImage;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await model.ImageFile.CopyToAsync(ms);

                    // Get content type automatically
                    var contentType = model.ImageFile.ContentType;

                    imageBase64 = $"data:{contentType};base64," +
                                  Convert.ToBase64String(ms.ToArray());
                }
                var entity = new mst_Announcement
                {
                    Id = model.Id ?? 0,
                    Title = model.Title,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    IsNew = model.IsNew,
                    ImageBase64 = imageBase64
                };
                result apiResult;
                string? guid = null;
                if (model.Id == null)
                {
                    guid = null;
                }
                else
                {
                    guid = model.Id.ToString();
                }
                // --- Permission Check (Reusable) ---
                var permissionResult = CommonHelper.HasPermission_AddEdit(guid, permission);
                if (!permissionResult.status)
                {
                    TempData["ErrorMessage"] = permissionResult.message;
                    model.AnnouncementList = await _sliderService.GetAllAnnouncement();
                    return View(model);
                }

                if (model.Id > 0)
                    apiResult = await _sliderService.UpdateAnnouncement(entity);   // 👈 UPDATE
                else
                    apiResult = await _sliderService.SaveAnnouncement(entity);     // 👈 INSERT

                TempData["Status"] = apiResult.status;
                TempData["SuccessMessage"] = apiResult.message;        

                return RedirectToAction(nameof(Announcement));
            }
            catch (Exception ex)
            {
                // Log exception in database
                await _iLog.ErrorInsertLog(
                    "WebsiteMaster",
                    "Post Announcement",
                    ex
                );
                TempData["ErrorMessage"] = "Something went wrong.";
                return View(model);
            }
        }
        #endregion
        #region delete & Active Deactive Announcement row
        [HttpPost]
        public async Task<IActionResult> deleteAnnouncementRow(int rowId)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            //int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/websitemaster/announcement";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            string? guid = null;
            if (rowId == null)
            {
                guid = null;
            }
            else
            {
                guid = rowId.ToString();
            }

            var permissionResult = CommonHelper.HasPermission_CanDelete(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);

            return Json(await _sliderService.deleteAnnouncementRow(rowId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAnnouncementStatus([FromBody] ToggleAnnouncement model)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
                return RedirectToAction("Login", "Account");

            string url = "/websitemaster/announcement";
            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            string? guid = null;
            if (model.Id == null)
            {
                guid = null;
            }
            else
            {
                guid = model.Id.ToString();
            }

            var permissionResult = CommonHelper.HasPermission_CanActive(guid, permission);
            if (!permissionResult.status)
                return Json(permissionResult);

            var result = await _sliderService.ToggleAnnouncementStatus(model.Id, model.IsActive);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAnnouncementIsNew([FromBody] ToggleAnnouncement model)
        {
            var result = await _sliderService.ToggleAnnouncementIsNew(model.Id, model.IsNew);
            return Json(result);
        }
     
        #endregion
    }
}
