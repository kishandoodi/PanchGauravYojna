using Microsoft.AspNetCore.Mvc;
using MO.Common;
using MO.GauravProfile;
using MO.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Progres
{
    public interface IGauravProfileAnswer
    {
        Task<result> SaveAnswersUsingProcedure(List<AnswerBlock> answers, string gauravId, int gauravDistrictId, int districtId, string createdBy);
        Task<List<AnswerBlock>> GetAnswersForEditAsync(string gauravId, int districtId, int gauravDistId);
        Task<result> GetGauravVivran(int District_Id);
        Task<result> SaveGauravVivran(List<GauravVivranModel> list, int districtId);
        Task<result> SaveGauravMainProfileAnswer(GauravMainProfileModel model, int districtId);
        
        Task<result> SaveGauravVivran_Final(List<GauravVivranModel> list, int districtId);
        Task<result> SaveGauravMainProfileAnswer_Final(GauravMainProfileModel model, int districtId);
        Task<result> Profile_step2_Final(string gauravId, int districtId);
        Task<result> GetStep2_Status(string gauravId, int districtId);
        #region Step1
        Task<result> GetStep1Questions(long districtId,string gauravGuid);
        Task<result> SaveStep1Answers(List<MainQuestionMO> answers,int financialYearId, long districtId,string gauravGuid, long userid);

        Task<result> GetStep2Questions(string gauravGuid);

        Task<result> SaveStep2Answers(List<Step2AnswerMO> answers,int financialYearId, long districtId,string gauravGuid, long userId, int rowId);
        Task<result> GetStep2Answers(int districtId, string gauravGuid, int financialYearId);
        Task<result> deleteStep2(string rowId, string guid,int districtId,int FyId);
        Task<result> GetStep3Questions(long districtId, string gauravGuid);
        #endregion

        #region main Profile
        Task<result> GetMainGauravProfile(int DistrictId, int FyId);
        Task<result> SaveMainProfile(mainProfileMO model,string submitType);
        #endregion

        #region Send profile verification
        Task<result> SendVerification_Profile(int financialYearId, long districtId, string gauravGuid, long userId);
        Task<result> DoneVerification_Profile(int financialYearId, long districtId, string gauravGuid, long userId,string remark);
        Task<result> RevertBackVerification_Profile(int financialYearId, long districtId, string gauravGuid, long userId,string remark);
        #endregion

        #region Pending Verification List
        Task<List<ProfileVerificationPendingVM>> GetProfileVerificationPendingAsync(int financialYearId, long districtId,int deptId);
        Task<ProfileBasicDetail> GetBasicDetail(int financialYearId, string gauravGuid, int districtId);
        #endregion
        #region District work Plan Trail
        Task<result> GetApplicationTrail(int financialYearId, string gauravGuid, int districtId);
        #endregion
        #region GetApplicationLastRemark
        Task<result> GetLastRemark(int financialYearId, string gauravGuid, int districtId);
        #endregion
    }
}
