using MO.Common;
using MO.GroupMaster;
using MO.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GauravMaster
{
    public interface IGauravMaster
    {
        #region Gaurav
        Task<result> SaveGaurav(GauravMasterMO model);
        Task<List<GauravMasterMO>> GetAllGaurav();
        Task<result> Delete(string guid);
        Task<GauravMasterMO> GetGauravByGuid(string guid);
        Task<result> ToggleActive(string guid);
        #endregion
        #region Gaurav Department
        Task<result> SaveGauravDepartment(GauravDepartmentMO model);
        Task<List<GauravDepartmentMO>> GetAllGauravDepartment();
        Task<result> GauravDepartmentDelete(string guid);        
        Task<result> GauravDepartmentToggleActive(string guid);
        #endregion

        #region Gaurav for Dashboard
        Task<List<GauravMasterMO>> GetAllGauravDashboard(int districtId, int FyId,int DeptId);
        #endregion
    }
}
