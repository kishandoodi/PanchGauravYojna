using MO.Common;
using MO.MenuMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ManageMaster
{
    public interface IMenu
    {
        #region Menu and SuMenu
        Task<result> saveMenu(MenuMO model);
        Task<result> saveSubMenu(MenuMO model);
        Task<ListData> MenudataList(int draw, string search, int pageIndex, int pageSize, string sortCol, string sortDir, int userId, CommonFilter _filter);
        Task<ListData> BindMenuData(DataSet ds, int draw, string sortCol, string sortDir);
        Task<result> IsActiveMenu(int MenuId);
        Task<result> DeleteMenu(string guid);
        Task<MenuMO> GetMenuDetailByGuid(string Guid);
        Task<MenuMO> GetSubMenuDetailByGuid(string Guid);
        Task<ListData> SubMenudataList(int draw, string search, int pageIndex, int pageSize, string sortCol, string sortDir, int userId, CommonFilter _filter);
        Task<result> BindDropDown_HeaderMenu();

        #endregion

        #region Menu Permission
        Task<List<MenuItem>> GetMenuPermissiondataList(CommonFilter _filter);
        Task<result> saveMenuPermissions(SaveGroupPermissionModel model);
        #endregion
    }
}
