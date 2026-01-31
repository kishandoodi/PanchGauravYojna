using MO.Common;
using MO.Master;
using MO.ProfileUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Common
{
    public interface ICommon
    {
        Task<result> BindGauravDropDown(string userId);
        Task<result> BindDistrictDropDown(string userId);

        Task<List<DistrictListDDL>> DDL_DistrictAsync();
        Task<List<RoleListDDL>> DDL_RoleAsync();
        Task<List<DepartmentListDDL>> DDL_DepartmentAsync();
        Task<List<UserLevelListDDL>> DDL_UserLevelAsync();
        Task<List<GauravDDL>> DDL_GauravAsync();
        Task<List<GroupDDL>> DDL_GroupAsync();
        Task<List<DistrictListDDL>> DDL_DistrictByUserIdAsync(string userId);
        Task<List<FinancialYearDDL>> DDL_FinancialYearAsync();
        Task<List<FinancialYearListMO>> FinancialYearList(FinancialYearListMO modal);
    }
}
