using MO.Common;
using MO.GroupMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GroupMaster
{
    public interface IGroupMaster
    {
        Task<result> SaveGroup(GroupMO model);
        Task<List<GroupMO>> GetAllGroup();       
        Task<GroupMO> GetGroupByGuid(string guid);
        Task<result> ToggleActive(string guid);
        Task<result> BindDropDown_Group();
        Task<result> Delete(string guid);
    }
}
