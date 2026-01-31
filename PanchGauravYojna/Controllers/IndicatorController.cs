using BL.Common;
using BL.Progres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MO;
using MO.Common;
using MO.Indicator;
using System.Security.Claims;
using static Azure.Core.HttpHeader;

namespace PanchGauravYojna.Controllers
{
   
    public class IndicatorController : Controller
    {
        private readonly IProgressReport _iProgressReport;
        private readonly IInputValidator _iInputValidator;
        private readonly IPermission _iPermission;
        #region Constructor
        public IndicatorController(IProgressReport iProgressReport,IInputValidator inputValidator, IPermission iPermission)
        {
            _iProgressReport = iProgressReport;
            _iInputValidator = inputValidator;
            _iPermission = iPermission;
        }
        #endregion

        //[Authorize(Roles = "SuperAdmin,Admin Des,Supervisor,Nodal Officer,Super Spervisor")]
        [Authorize(Policy = "PageAccess")]
        public async Task<IActionResult> Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }

            int groupId = Convert.ToInt32(User.FindFirstValue("GroupId"));
            string url = HttpContext.Request.Path.Value ?? "";

            var permission = await _iPermission.GetPagePermissionAsync(groupId, url);

            //if (permission == null || !permission.CanList)
            //    return Forbid(); // no list permission

            ViewBag.Permission = permission ?? new MenuPermissionMO();

            return View();
        }

        #region indicator page
        //[Authorize(Policy = "PageAccess")]
        public IActionResult IndicatorMaster()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            return View();
        }
        
        [HttpGet]
        [Route("Indicator/GetProgressReport")]
        public async Task<IActionResult> GetProgressReport(Indicatorreport model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            // Access current logged-in user's claims
            var isAuthenticated = User.Identity.IsAuthenticated;
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);            
            var ssoid = User.FindFirst("SSOID")?.Value;
            if (!isAuthenticated)
            {
                return RedirectToAction("LogOut", "Account");
            }
            if (districtId == 0)
                districtId =(int)model.districtId;
            result _res = await _iProgressReport.GetProgressReportNew(model.Id.ToString(), districtId);
            
            return Json(_res);        
        }           
        
        [HttpPost]       
        public async Task<JsonResult> SaveIndicatorTargetMaster([FromBody] List<IndicatorTargetMaster> model)
        {
            if (!_iInputValidator.ValidateInput(model).status)
            {
                return Json(_iInputValidator.ValidateInput(model));
            }
            else
            {
                return Json(await _iProgressReport.SaveIndicatorTargetMaster(model));
            }           
            //return Json(result);
        }
        #endregion


        #region GauravDetail       
        public IActionResult GauravDetail()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            return View();
            
        }
        //[Route("Indicator/GetGauravQuestion")]
        public async Task<IActionResult> GetGauravQuestion(string gauravId, string? districtId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            // Access current logged-in user's claims
            var isAuthenticated = User.Identity.IsAuthenticated;
            // Get DistrictId claim as string and safely convert to int
            var roleid = User.FindFirst("RoleId")?.Value;
            int District_Id = 0;
            if (roleid == "4" || roleid == "3")
            {
                var districtIdClaim = User.FindFirst("DistrictId")?.Value;

                if (!string.IsNullOrEmpty(districtIdClaim))
                {
                    int.TryParse(districtIdClaim, out District_Id);
                }
            }
            else if (roleid == "2")
            {
                District_Id = Convert.ToInt32(districtId);
            }
            var ssoid = User.FindFirst("SSOID")?.Value;

            if (!isAuthenticated)
            {
                return RedirectToAction("LogOut", "Account");
            }
            result _res = await _iProgressReport.GetQuestionReport(gauravId, District_Id);
            _res.source = User.FindFirst("Role")?.Value;
            return Json(_res);
        }
        #endregion

        #region Crop Gaurav
     
        public IActionResult CropGaurav()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized(); // Returns a 401 Unauthorized response
            }
            return View();
        }
        #endregion

        [HttpPost]
        //[Route("Indicator/SaveGauravAnswers")]
        public async Task<JsonResult> SaveGauravAnswers([FromBody] List<AnswerBlock> model)
        {
            if (model == null)
                return Json(new { status = false, message = "No data received." });

            // Do something with model
            return Json(new { status = true, message = "Data received successfully!" });
        }
    }
}
