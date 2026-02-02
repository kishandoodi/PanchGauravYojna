using BL.WebsiteMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        #region Announcement
        public async Task<IActionResult> Announcement()
        {
            var model = new Announcement();
            model.AnnouncementList = await _sliderService.GetAllSliderImage();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveAnnouncement(Announcement model)
        {
            string filePath = model.ExistingFilePath;
            string fileType = "";

            if (model.File != null)
            {
                string ext = Path.GetExtension(model.File.FileName).ToLower();
                fileType = ext == ".pdf" ? "pdf" : "image";

                string folder = Path.Combine(_env.WebRootPath, "uploads/announcements");
                Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + ext;
                filePath = "/uploads/announcements/" + fileName;

                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await model.File.CopyToAsync(stream);
            }


            return RedirectToAction("Index");
        }

        #endregion

    }
}
