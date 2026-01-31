using MO.Common;
using MO.ProfileUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ProfileUser
{
    public interface IProfileUser
    {
        Task<result> SaveUserAsync(UserMO model);
        Task<List<UserListMO>> GetUserListAsync();
        Task<UserMO> GetUserByGuidAsync(string guid);
        Task<result> ActiveDeactiveUserAsync(string guid,string modifiedBy);
        Task<result> Delete(string guid, string modifiedBy);
    }
}
