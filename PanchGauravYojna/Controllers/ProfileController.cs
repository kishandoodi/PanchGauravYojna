using BL.Common;
using BL.GauravMaster;
using BL.Progres;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MO.Common;
using MO.GauravProfile;
using MO.Master;
using PanchGauravYojna.Helpers;
using PanchGauravYojna.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace PanchGauravYojna.Controllers
{
    public class ProfileController : Controller
    {       
        private readonly IGauravProfileAnswer _igauravProfileAnswer;
        private readonly IGauravMaster _iIGauravMaster;
        private readonly ICommon _iCommon;
        private readonly IPermission _iPermission;
        #region Constructor
        public ProfileController(IGauravProfileAnswer igauravProfileAnswer,
            IGauravMaster iIGauravMaster,ICommon iCommon,IPermission iPermission)
        {          
            _igauravProfileAnswer = igauravProfileAnswer;
            _iIGauravMaster = iIGauravMaster;
            _iCommon = iCommon;
            _iPermission = iPermission;
        }
        #endregion

        #region Dashboard
        [Authorize(Policy = "PageAccess")]
        public async Task<IActionResult> Dashboard()
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            int DeptId = Convert.ToInt32(User.FindFirst("DepartmentId").Value);
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            // FINAL MODEL
            List<GauravProfileDashboardMO> model = new List<GauravProfileDashboardMO>();

            // GET BOTH LISTS
            var GauravDept_list = await _iIGauravMaster.GetAllGauravDepartment();
            //var Gaurav_list = await _iIGauravMaster.GetAllGaurav();
            var Gaurav_list = await _iIGauravMaster.GetAllGauravDashboard(districtId, FyId);

            List<GauravMasterMO> filteredGaurav;

            if (DeptId == 0)
            {
                filteredGaurav = Gaurav_list.ToList();  // No filtering
            }
            else
            {
                filteredGaurav = (from g in Gaurav_list
                                  join d in GauravDept_list
                                  on g.GauravId equals d.GauravId
                                  where d.DepartmentId == DeptId
                                  select g).ToList();
            }
            //if()
            //// FILTER GAURAV LIST BY LOGGED-IN DEPARTMENT
            //var filteredGaurav = (from g in Gaurav_list
            //                      join d in GauravDept_list
            //                      on g.GauravId equals d.GauravId
            //                      where d.DepartmentId == DeptId
            //                      select g).ToList();

            // MAP TO PROFILE MODEL
            foreach (var item in filteredGaurav)
            {
                model.Add(new GauravProfileDashboardMO
                {
                    DepartmentId = DeptId,
                    GauravGuid = item.Guid,
                    GauravName = item.Name,
                    GauravId = (int)item.GauravId,
                    StatusId = item.StatusId
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckAndRedirect(string guid, int gauravId,string statusClass)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);

            // Call your service
            var result = await _igauravProfileAnswer.GetStep2_Status(guid, districtId);

            if (result.status)
            {
                // If step-2 exists → redirect to print preview
                return RedirectToAction("PrintPreview", new { guid = guid });
            }
            else
            {
                // If step-2 does not exist → redirect to profile form
                return RedirectToAction("Step1", new { guid = guid , statusClass = statusClass });
            }
        }
        #endregion

        #region main profile
        [Authorize(Policy = "PageAccess")]
        public async Task<IActionResult> MainProfile()
        {
                //mainProfileMO model = new mainProfileMO();
                var userLevel = Convert.ToInt32(User.FindFirst("UserLevel")?.Value);
                var department = Convert.ToInt32(User.FindFirst("UserLevel")?.Value);
                var district = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
                ViewBag.districtId = district;
                result rs = await _igauravProfileAnswer.GetMainGauravProfile(district,2);
                return View(rs.data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveMainProfile(mainProfileMO model,string submitType)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            bool isFinalSubmit = submitType == "Verify"; // When user clicks Send for Verification
            model.CreatedBy = userId.ToString();

            // Save Step
            var result = await _igauravProfileAnswer.SaveMainProfile(model,submitType);
            //return RedirectToAction("MainProfile", new { districtId = model.DistrictId });
            if (result.status)
            {
                TempData["SuccessMessage"] = "Profile saved successfully!";
                return RedirectToAction("MainProfile");
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong!";
                return View(model);
            }
        }        
        #endregion

        #region Step1
        [HttpGet]
        public async Task<IActionResult> Step1(string guid,string statusClass)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);

            Step1MO model = new Step1MO();
            model.Questions = new List<MainQuestionMO>();
            model.CurrentStep = 1;
            model.GauravGuid = guid;
            var _questionlist = await _igauravProfileAnswer.GetStep1Questions(districtId,guid);
            model.Questions = _questionlist.data;
            model.statusClass = statusClass;
            // model.LastRemark = _questionlis;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Step1(Step1MO model)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            //string guid = Request.Query["guid"];
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            // List that will store all answers
            List<MainQuestionMO> answers = new();

            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("Answer_"))
                {
                    // Extract QuestionMasterId
                    string idPart = key.Replace("Answer_", "");
                    if (Int32.TryParse(idPart, out Int32 questionId))
                    {
                        string answerText = Request.Form[key];

                        answers.Add(new MainQuestionMO
                        {
                            QuestionMasterId = questionId,
                            AnswerValue = answerText,
                        });
                    }
                }
            }
            //Now save using your BL
            var saveResult = await _igauravProfileAnswer.SaveStep1Answers(answers, FyId, districtId, model.GauravGuid, userId);

            if (!saveResult.status)
            {
                TempData["Error"] = saveResult.message;
                return RedirectToAction("Steps1");
            }

            TempData["Success"] = "Step 1 answers saved successfully!";

            // Redirect to Step 2
            return RedirectToAction("Step2", new { guid = model.GauravGuid });
        }
        #endregion

        #region Step2
        [HttpGet]
        public async Task<IActionResult> Step2(string guid)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            Step2MO model = new Step2MO();
            model.CurrentStep = 2;
            model.GauravGuid = guid;
            result res_model = await _igauravProfileAnswer.GetStep2Questions(guid);
            model.mainquestion = res_model.data;
            result _res_data= await _igauravProfileAnswer.GetStep2Answers(districtId, guid, FyId);
            ViewBag.List = _res_data.data;
            ViewBag.ActivityTypes = Enum.GetValues(typeof(EnumProfileQuestion))
                .Cast<EnumProfileQuestion>()
                .Select(x => new SelectListItem
                {
                    Text = GetDisplayName(x),
                    Value = ((int)x).ToString()
                })
                .ToList();
            //model.Rows = new List<Step2RowMO>();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Step2([FromBody] List<Step2AnswerMO> answers,string guid,int rowId=0)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            string gauravGuid = guid;
            var result = await _igauravProfileAnswer.SaveStep2Answers(answers,FyId,districtId,guid,userId,rowId);

            if (!result.status)
            {
                TempData["Error"] = result.message;
                return RedirectToAction("Steps1");
            }

            TempData["Success"] = "Step 1 answers saved successfully!";

            // Redirect to Step 2
            return RedirectToAction("Step3", new { guid = guid });
        }
        
        public async Task<IActionResult> Complete_Step2(string guid)
        {
            return RedirectToAction("Step3", new { guid = guid });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteStep2(string rowId,string guid)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            return Json(await _igauravProfileAnswer.deleteStep2(rowId, guid,districtId,FyId));
        }
        #endregion

        #region Step3
        [HttpGet]
        public async Task<IActionResult> Step3(string guid)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);

            Step3MO model = new Step3MO();           
            model.CurrentStep = 3;
            model.GauravGuid = guid;
            var _questionlist = await _igauravProfileAnswer.GetStep3Questions(districtId, guid);
            model.mainQuestionStep3MOs = _questionlist.data;
            return View(model);
        }
       
        [HttpPost]
        public async Task<IActionResult> Step3(Step3MO model, string submitType)
        {
            int districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            if (submitType == "Save")
            {
                List<MainQuestionMO> answers = new List<MainQuestionMO>();

                // Loop all form values
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("Answer_"))
                    {
                        string qIdString = key.Replace("Answer_", "");
                        if (int.TryParse(qIdString, out int questionMasterId))
                        {
                            string value = Request.Form[key];

                            if (!string.IsNullOrEmpty(value))
                            {
                                answers.Add(new MainQuestionMO
                                {
                                    QuestionMasterId = questionMasterId,
                                    AnswerValue = value,
                                    //CreatedBy = userId
                                });
                            }
                        }
                    }
                }

                if (!answers.Any())
                {
                    TempData["Error"] = "Please fill at least one answer.";
                    return RedirectToAction("Step3", new { guid = model.GauravGuid });
                }

                //Now save using your BL
                var saveResult = await _igauravProfileAnswer.SaveStep1Answers(answers, FyId, districtId, model.GauravGuid, userId);

                if (!saveResult.status)
                {
                    TempData["Error"] = saveResult.message;
                    return RedirectToAction("Steps1");
                }

                TempData["Success"] = "Step 1 answers saved successfully!";

                // Redirect to Step 2
                return RedirectToAction("Step3", new { guid = model.GauravGuid });
            }
            else
            {
                //Now save using your BL
                var saveResult = await _igauravProfileAnswer.SendVerification_Profile(FyId, districtId, model.GauravGuid, userId);
                if (!saveResult.status)
                {
                    TempData["Error"] = saveResult.message;
                    return RedirectToAction("Step3", new { guid = model.GauravGuid });
                }
                TempData["Success"] = saveResult.message;
                return RedirectToAction("PrintPreview", new { guid = model.GauravGuid });
            }
        }
        #endregion

        #region GetApplicationTrail
        [HttpGet]
        public async Task<IActionResult> ViewTrail(string gauravGuid)
        {
            int FYId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            int district = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);

            var response =
                await _igauravProfileAnswer.GetApplicationTrail(
                    FYId, gauravGuid, district);

            if (response.status)
            {
                return PartialView("_ViewProfileTrails",
                    response.data as List<ProfileBasicTrails>);
            }

            return PartialView("_ViewProfileTrails",
                new List<ProfileBasicTrails>());
        }
        #endregion

        public async Task<IActionResult> PrintPreview(string guid)
        {            
            PrintProfile model = new PrintProfile();
            long districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            model.GauravGuid = guid;
            // step1
            var step1 = await _igauravProfileAnswer.GetStep1Questions(districtId, guid);
            model.step1 = step1.data;
            //step2
            result _res_data = await _igauravProfileAnswer.GetStep2Answers(Convert.ToInt32(districtId), guid,2);
            ViewBag.List = _res_data.data;
            //step3
            var step3 = await _igauravProfileAnswer.GetStep3Questions(districtId, guid);
            model.step3 = step3.data;
            return View(model);
        }

        #region ViewProfile for State Level

        [HttpGet]
        [Authorize(Policy = "PageAccess")]
        public async Task<IActionResult> PendingProfileVerification()
        {
            long districtId = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);
            int deptId = Convert.ToInt32(User.FindFirst("DepartmentId")?.Value);
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
            //ViewBag.DistrictList = await _iCommon.DDL_DistrictByUserIdAsync(userId.ToString());
            List<ProfileVerificationPendingVM> _list =await _igauravProfileAnswer.GetProfileVerificationPendingAsync(2, districtId, deptId);
            return View(_list);
        }
        [HttpGet]
        public async Task<IActionResult> ViewProfile(string gauravGuid, long district)
        {
            PrintProfile model = new PrintProfile();
            int FyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);

            model.GauravGuid = gauravGuid;

            // get basic deatil 
            ProfileBasicDetail _basicdeatil = await _igauravProfileAnswer.GetBasicDetail(FyId, gauravGuid, Convert.ToInt32(district));
            model.District = _basicdeatil.District;
            model.FinancialYear = _basicdeatil.FinancialYear;
            model.Gaurav = _basicdeatil.GauravEng;
            model.GauravItem = _basicdeatil.GauravItemEng;
            model.DistrictId = _basicdeatil.DistrictId;
            // Step 1
            var step1 = await _igauravProfileAnswer.GetStep1Questions(district, gauravGuid);
            model.step1 = step1.data;

            // Step 2
            result _res_data = await _igauravProfileAnswer.GetStep2Answers(
                                    Convert.ToInt32(district), gauravGuid, FyId);
            ViewBag.List = _res_data.data;
            //model.step2 = _res_data.data;

            // Step 3
            var step3 = await _igauravProfileAnswer.GetStep3Questions(district, gauravGuid);
            model.step3 = step3.data;
            // ✅ Get Last Remark
            var lastRemarkResponse =
                await _igauravProfileAnswer.GetLastRemark(
                    FyId, gauravGuid, Convert.ToInt32(district));

            model.LastRemark = lastRemarkResponse.data?.ToString() ?? "";

            return PartialView("_ViewProfilePopup", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevertBackVerification([FromBody] ProfileVerifyActionVM model)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
                return Json(new { status = false, message = "Invalid request" });

            //long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var res = await _igauravProfileAnswer.RevertBackVerification_Profile(
                            Convert.ToInt32(model.FinancialYearId),
                            Convert.ToInt64(model.DistrictId),
                            model.GauravGuid,
                            userId,
                            model.Remark);

            return Json(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyProfile([FromBody] ProfileVerifyActionVM model)
        {
            if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("Login", "Account");
            }
            if (!ModelState.IsValid)
                return Json(new { status = false, message = "Invalid request" });

            //long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var res = await _igauravProfileAnswer.DoneVerification_Profile(
                           Convert.ToInt32(model.FinancialYearId),
                            Convert.ToInt64(model.DistrictId),
                            model.GauravGuid,
                            userId,
                            model.Remark);

            return Json(res);
        }
        #endregion

        #region common
        public static string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName()
                ?? enumValue.ToString();
        }
        #endregion

        #region GetApplicationLastRemark
        [HttpGet]
        public async Task<IActionResult> GetLastRemark(string gauravGuid)
        {
            int fyId = Convert.ToInt32(User.FindFirst("FinancialYear")?.Value);
            int district = Convert.ToInt32(User.FindFirst("DistrictId")?.Value);

            var response = await _igauravProfileAnswer
                .GetLastRemark(fyId, gauravGuid, district);

            return Json(new
            {
                status = response.status,
                lastRemark = response.data?.ToString() ?? ""
            });
        }


        #endregion

        
    }
}
