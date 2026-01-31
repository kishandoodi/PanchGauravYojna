using MO;
using MO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Progres
{
    public interface IProgressReport
    {
        Task<result> GetProgressReportNew(string Id,int District_Id);
        Task<result> SaveIndicatorTargetMaster(List<IndicatorTargetMaster> model);
        Task<result> GetQuestionReport(string gauravId, int District_Id);
        Task<result> saveGauravProfile(int gauravid, int distid, int gauravdistid, int financialyearid, string questionId, string answer, string type, string column = null, string createdBy = "System");
        Task<result> GetQuestionReport_New(string gauravId, int District_Id);
        Task<result> GetQuestionReport_New1(string gauravId, int District_Id);


    }
}
