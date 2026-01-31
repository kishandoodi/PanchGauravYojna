using MO.Common;
using MO.GroupMaster;
using MO.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Department
{
    public interface IDepartment
    {
        Task<result> SaveDepartment(DepartmentMO model);
        Task<List<DepartmentMO>> GetAllDepartment();
        Task<result> Delete(string guid);
        Task<GroupMO> GetGroupByGuid(string guid);
        Task<result> ToggleActive(string guid);
    }
}
