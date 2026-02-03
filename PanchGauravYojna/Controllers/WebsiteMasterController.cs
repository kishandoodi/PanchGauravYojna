using BL.WebsiteMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.Common;
using MO.WebsiteMaster;
using System.Threading.Tasks;

namespace PanchGauravYojna.Controllers
{
    public class WebsiteMasterController : Controller       
    {
        #region properties
        private readonly IWebHostEnvironment _env;
        private readonly ISliderVM _sliderService;
        #endregion
        #region Constructor
        public WebsiteMasterController(IWebHostEnvironment env , ISliderVM sliderService)
        {
            _env = env;
            _sliderService = sliderService;
        }
        #endregion
        #region slider image get,save & update
        [HttpGet]
        public async Task<IActionResult> SliderVM()
        {
            var model = new SliderCreateVM();
            model.SliderList = await _sliderService.GetAllSliderImage();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SliderVM(SliderCreateVM model)
        {


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

            if (model.Id > 0)
                await _sliderService.UpdateSliderAsync(entity);   // 👈 UPDATE
            else
                await _sliderService.SaveSliderAsync(entity);     // 👈 INSERT

            return RedirectToAction(nameof(SliderVM));
        }


        #endregion
        #region delete & Active Deactive slider image row
        [HttpPost]
        public async Task<IActionResult> deleteSliderRow(int rowId)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            return Json(await _sliderService.deleteSliderRow(rowId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSliderStatus([FromBody] ToggleSliderVM model)
        {
            var result = await _sliderService.ToggleStatusAsync(model.Id, model.IsActive);
            return Json(result);
        }

        #endregion
        #region Announcement list save & update
        public async Task<IActionResult> Announcement()
        {
            var model = new Announcement();
            model.AnnouncementList = await _sliderService.GetAllAnnouncement();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Announcement(Announcement model)
        {
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
            if (model.Id > 0)
                apiResult =await _sliderService.UpdateAnnouncement(entity);   // 👈 UPDATE
            else
                apiResult =await _sliderService.SaveAnnouncement(entity);     // 👈 INSERT

            TempData["Status"] = apiResult.status;
            TempData["Message"] = apiResult.message;

            return RedirectToAction(nameof(Announcement));
        }


        #endregion
        #region delete & Active Deactive Announcement row
        [HttpPost]
        public async Task<IActionResult> deleteAnnouncementRow(int rowId)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            return Json(await _sliderService.deleteAnnouncementRow(rowId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAnnouncementStatus([FromBody] ToggleAnnouncement model)
        {
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
