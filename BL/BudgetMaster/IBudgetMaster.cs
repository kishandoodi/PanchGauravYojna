using Microsoft.AspNetCore.Mvc;
using MO.BudgetMaster;
using MO.Common;
using MO.GauravProfile;
using MO.WebsiteMaster;

namespace BL.BudgetMaster
{
    public interface IBudgetMaster
    {
        Task<result> BindGauravDropDownVetting(string userId);
        Task<result> BindDistrictDropDownVetting();
        Task<result> GetPendingList(int garauvId, int districtId, int FyId);
        Task<result> GetPendingVettingList(int RawId, int garauvId, int districtId, int FyId, int SubQuestionMasterId, int QuestionMasterId);
        Task<result> SaveVettingData(VettingSaveVM obj);
        Task<result> GetPendingVettingList(VettingSaveVM obj);
        Task<result> DeleteVettedList(int RawId, int garauvId, int DistrictId,int FyId, int SubQuestionMasterId, int QuestionMasterId);
        Task<result> UpdateVettedList(VettingSaveVM obj);
        Task<result> GetVettedQuestions(string GauravId);
        Task<result> savevettedquestions(VettedQuestionsSaveModel obj, int financialYearId, int DistrictId, int GauravId, long userId);

    }
}