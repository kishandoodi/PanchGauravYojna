using BL.BudgetMaster;
using BL.Progres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MO.BudgetMaster;
using MO.WebsiteMaster;

namespace PanchGauravYojna.Controllers
{
    public class BudgetController : Controller
    {
        #region properties
        private readonly IBudgetMaster _iBudgetMaster;

        #endregion
        #region Constructor
        public BudgetController(IBudgetMaster iBudgetMaster)
        {
            _iBudgetMaster = iBudgetMaster;
        }
        #endregion

        public IActionResult Vetting()
        {
            return View();
        }
        public async Task<IActionResult> BindGauravDropDown()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            string userId = User.FindFirst("UserId")?.Value;
            return Json(await _iBudgetMaster.BindGauravDropDownVetting(userId));
        }
        public async Task<IActionResult> BindDistrictDropDownAll()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            string userId = User.FindFirst("UserId")?.Value;
            return Json(await _iBudgetMaster.BindDistrictDropDownVetting());
        }
        [HttpPost]
        public async Task<IActionResult> GetVettingList(int garauvId, int districtId)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            var result = await _iBudgetMaster.GetVettingList(garauvId, districtId, FyId);

            return PartialView("_VettingList", result.data);
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingVettingList(int RawId, int garauvId,int DistrictId,int SubQuestionMasterId,int QuestionMasterId)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            //int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            ViewBag.ShowVerifyButton = false;
            var result = await _iBudgetMaster.GetPendingVettingList(RawId, garauvId, DistrictId, FyId, SubQuestionMasterId, QuestionMasterId);

            return PartialView("_VettingList", result.data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveVettingData(List<VettingSaveVM> items)
        {
            await _iBudgetMaster.SaveVettingData(items);
            return Json(true);
        }

    }
}
