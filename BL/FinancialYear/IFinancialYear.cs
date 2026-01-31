using MO.Common;
using MO.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.FinancialYear
{
    public interface IFinancialYear
    {
        Task<result> SaveFinancialYear(FinancialYearCM model);
        Task<List<FinancialYearCM>> GetAllFinancialYear();
        Task<result> ToggleActive(string guid);
        Task<result> IsCurrent(string guid);
        Task<result> Delete(string guid);

    }
}
