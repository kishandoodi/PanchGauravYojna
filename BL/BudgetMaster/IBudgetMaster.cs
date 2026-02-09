using Microsoft.AspNetCore.Mvc;
using MO.BudgetMaster;
using MO.Common;
using MO.WebsiteMaster;

namespace BL.BudgetMaster
{
    public interface IBudgetMaster
    {
        Task<result> BindGauravDropDownVetting(string userId);
        Task<result> BindDistrictDropDownVetting();
        Task<result> GetVettingList(int garauvId, int districtId, int FyId);
        Task<result> GetPendingVettingList(int RawId, int garauvId, int districtId, int FyId, int SubQuestionMasterId, int QuestionMasterId);
        Task<result> SaveVettingData(List<VettingSaveVM> items);

    }
}