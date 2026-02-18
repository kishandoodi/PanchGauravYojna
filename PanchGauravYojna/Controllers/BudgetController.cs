using BL.BudgetMaster;
using BL.Progres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MO.BudgetMaster;
using MO.Common;
using MO.GauravProfile;
using MO.WebsiteMaster;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
        public async Task<IActionResult> GetPendingList(int garauvId, int districtId)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            var result = await _iBudgetMaster.GetPendingList(garauvId, districtId, FyId);

            return PartialView("_VettingList", result.data);
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingVettingList(int RawId, int garauvId, int DistrictId, int SubQuestionMasterId, int QuestionMasterId)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            //int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            ViewBag.ShowVerifyButton = false;
            var result = await _iBudgetMaster.GetPendingVettingList(RawId, garauvId, DistrictId, FyId, SubQuestionMasterId, QuestionMasterId);

            return PartialView("_VettingList", result.data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveVettingData(VettingSaveVM obj)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            obj.FinancialYear_Id = FyId;

            var result = await _iBudgetMaster.SaveVettingData(obj);

            return Json(result);
        }
        public async Task<IActionResult> GetPendingVettingList(VettingSaveVM obj)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            obj.FinancialYear_Id = FyId;
            var result = await _iBudgetMaster.GetPendingVettingList(obj);

            if (result.status)
                return PartialView("_PendingVettingList",
                    (List<VettingSaveVM>)result.data);

            return PartialView("_PendingVettingList", null);
        }
        public async Task<IActionResult> DeleteVettedList(int RawId, int garauvId, int DistrictId, int SubQuestionMasterId,int QuestionMasterId)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            var result = await _iBudgetMaster.DeleteVettedList(RawId, garauvId, DistrictId, FyId, SubQuestionMasterId, QuestionMasterId);

            return Json(result);
        }
        public async Task<IActionResult> UpdateVettedList(VettingSaveVM obj)
        {
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            obj.FinancialYear_Id = FyId;
            var result = await _iBudgetMaster.UpdateVettedList(obj);

            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> VettedQuestions(string GauravId)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            VettedQuestions model = new VettedQuestions();
           // model.CurrentStep = 2;
            model.GauravGuid = GauravId;
            result res_model = await _iBudgetMaster.GetVettedQuestions(GauravId);
            model.mainquestion = res_model.data;
            ViewBag.ActivityTypes = Enum.GetValues(typeof(EnumProfileQuestion))
               .Cast<EnumProfileQuestion>()
               .Select(x => new SelectListItem
               {
                   Text = GetDisplayName(x),
                   Value = ((int)x).ToString()
               })
               .ToList();
            // return partial so client can inject into existing page and show modal
            return PartialView("_VettedQuestionList", model);
        }
        public static string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName()
                ?? enumValue.ToString();
        }
        [HttpPost]
        public async Task<IActionResult> savevettedquestions([FromBody] VettedQuestionsSaveModel obj, int DistrictId, int GauravId)
        {
            //int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            //int gauravGuid = GauravId;
            var result = await _iBudgetMaster.savevettedquestions(obj, FyId, DistrictId, GauravId, userId);

            return Json(result);
        }

    }
}