using Azure.Core;
using DL;
using MO.Common;
using MO.MenuMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ManageMaster
{
    public class Menu :IMenu
    {
        #region Properties
        private readonly ISQLHelper _iSql;

        #endregion
        #region Constructor
        public Menu(ISQLHelper iSql)
        {
            _iSql = iSql;

        }
        #endregion

        #region Menu
        public async Task<result> saveMenu(MenuMO model)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                if (model.Guid != null) { param.Add(new SqlParameter("@Action", "UpdateMenu")); }
                else { param.Add(new SqlParameter("@Action", "InsertMenu")); }
                param.Add(new SqlParameter("@Guid", model.Guid));
                param.Add(new SqlParameter("@Name", model.Name));
                param.Add(new SqlParameter("@MangalName", model.MangalName));
                param.Add(new SqlParameter("@Controller", model.Controller));
                param.Add(new SqlParameter("@ActionName", model.ActionName));
                param.Add(new SqlParameter("@Icon", model.Icon));
                param.Add(new SqlParameter("@OrderNumber", model.OrderNumber));
                param.Add(new SqlParameter("@IsActive", model.IsActive));
                param.Add(new SqlParameter("@CreatedBy", ""));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                        message = Convert.ToString(ds.Tables[0].Rows[0]["message"])
                    };
                }
                else
                {
                    return new result
                    {
                        status = false,
                        message = "Something went wrong",
                    };
                }
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong",
                };
            }
        }
        public async Task<result> saveSubMenu(MenuMO model)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                if (model.Guid != null) { param.Add(new SqlParameter("@Action", "UpdateMenu")); }
                else { param.Add(new SqlParameter("@Action", "InsertMenu")); }
                param.Add(new SqlParameter("@Guid", model.Guid));
                param.Add(new SqlParameter("@MainMenuId", model.HeaderMenu));
                param.Add(new SqlParameter("@Name", model.Name));
                param.Add(new SqlParameter("@MangalName", model.MangalName));
                param.Add(new SqlParameter("@Controller", model.Controller));
                param.Add(new SqlParameter("@ActionName", model.ActionName));
                param.Add(new SqlParameter("@Icon", model.Icon));
                param.Add(new SqlParameter("@OrderNumber", model.OrderNumber));
                param.Add(new SqlParameter("@IsSubMenu", model.IsSubMenu));
                param.Add(new SqlParameter("@IsActive", model.IsActive));
                param.Add(new SqlParameter("@CreatedBy", ""));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                        message = Convert.ToString(ds.Tables[0].Rows[0]["message"])
                    };
                }
                else
                {
                    return new result
                    {
                        status = false,
                        message = "Something went wrong",
                    };
                }
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong",
                };
            }
        }
        public async Task<ListData> MenudataList(int draw, string search, int pageIndex, int pageSize, string sortCol, string sortDir, int userId, CommonFilter _filter)
        {
            ListData listData = new ListData();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Action", "GetListMenu"));
                parameters.Add(new SqlParameter("@PageIndex", pageIndex));
                parameters.Add(new SqlParameter("@PageSize", pageSize));
                parameters.Add(new SqlParameter("@groupId", _filter.groupId));
                parameters.Add(new SqlParameter("@url", _filter.url));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", parameters.ToArray());
                listData = await BindMenuData(ds, draw, sortCol, sortDir);
                return listData;
            }
            catch (Exception ex)
            {
                return new ListData();
            }
        }
        public async Task<ListData> BindMenuData(DataSet ds, int draw, string sortCol, string sortDir)
        {
            int totalRecord = 0;
            ListData listData = new ListData();
            List<MenuList> list = new List<MenuList>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataView dv = new DataView(ds.Tables[0]);
                    //dv.Sort = sortCol + " " + sortDir;
                    totalRecord = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRow"].ToString());
                    list = dv.ToTable().AsEnumerable().Select(s => new MenuList
                    {
                        SrNo = s["SrNo"] == DBNull.Value ? 0 : Convert.ToInt32(s["SrNo"]),
                        MenuId = s["MenuId"] == DBNull.Value ? 0 : Convert.ToInt32(s["MenuId"]),
                        MenuName = s["Name"] == DBNull.Value ? "" : Convert.ToString(s["Name"]),
                        Controller = s["Controller"] == DBNull.Value ? "" : Convert.ToString(s["Controller"]),
                        ActionName = s["ActionName"] == DBNull.Value ? "" : Convert.ToString(s["ActionName"]),
                        MangalName = s["MangalName"] == DBNull.Value ? "" : Convert.ToString(s["MangalName"]),
                        OrderNumber = s["OrderNumber"] == DBNull.Value ? "" : Convert.ToString(s["OrderNumber"]),
                        Icon = s["Icon"] == DBNull.Value ? "" : Convert.ToString(s["Icon"]),
                        IsActive = s["IsActive"] == DBNull.Value ? "" : Convert.ToString(s["IsActive"]),
                        Action = s["Action"] == DBNull.Value ? "" : Convert.ToString(s["Action"]),
                    }).ToList();
                }
            }
            listData.recordsFiltered = totalRecord;
            listData.recordsTotal = totalRecord;
            listData.data = list;
            listData.draw = draw;
            return listData;
        }
        public async Task<result> IsActiveMenu(int MenuId)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "IsActiveMenu"));
                param.Add(new SqlParameter("@MenuId", MenuId));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = true,
                        message = ds.Tables[0].Rows[0]["message"].ToString(),
                    };
                }
                else
                {
                    return new result
                    {
                        status = false,
                        message = "Something went wrong",
                    };
                }
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong",
                };
            }
        }
        public async Task<result> DeleteMenu(string guid)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "Delete"));
                param.Add(new SqlParameter("@Guid", guid));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = true,
                        message = ds.Tables[0].Rows[0]["message"].ToString(),
                    };
                }
                else
                {
                    return new result
                    {
                        status = false,
                        message = "Something went wrong",
                    };
                }
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong",
                };
            }
        }
        public async Task<MenuMO> GetMenuDetailByGuid(string Guid)
        {
            try
            {
                MenuMO targetMO = new MenuMO();
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetMenuByGuid"));
                param.Add(new SqlParameter("@Guid", Guid));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new MenuMO
                    {
                        Guid = ds.Tables[0].Rows[0]["Guid"].ToString(),
                        MenuId = ds.Tables[0].Rows[0]["MenuId"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["MenuId"]),
                        Name = ds.Tables[0].Rows[0]["Name"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Name"].ToString(),
                        MangalName = ds.Tables[0].Rows[0]["MangalName"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["MangalName"].ToString(),
                        Controller = ds.Tables[0].Rows[0]["Controller"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Controller"].ToString(),
                        ActionName = ds.Tables[0].Rows[0]["ActionName"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["ActionName"].ToString(),
                        OrderNumber = ds.Tables[0].Rows[0]["OrderNumber"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["OrderNumber"]),
                        Icon = ds.Tables[0].Rows[0]["Icon"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Icon"].ToString(),
                        IsActive = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"]),
                    };
                }
                else
                {
                    return new MenuMO
                    {
                    };
                }
            }
            catch (Exception ex)
            {
                return new MenuMO
                {
                };
            }
        }
        public async Task<MenuMO> GetSubMenuDetailByGuid(string Guid)
        {
            try
            {
                MenuMO targetMO = new MenuMO();
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetSubMenuByGuid"));
                param.Add(new SqlParameter("@Guid", Guid));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new MenuMO
                    {
                        Guid = ds.Tables[0].Rows[0]["Guid"].ToString(),
                        MenuId = ds.Tables[0].Rows[0]["MenuId"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["MenuId"]),
                        HeaderMenu = ds.Tables[0].Rows[0]["MainMenuId"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["MainMenuId"]),
                        Name = ds.Tables[0].Rows[0]["Name"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Name"].ToString(),
                        MangalName = ds.Tables[0].Rows[0]["MangalName"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["MangalName"].ToString(),
                        Controller = ds.Tables[0].Rows[0]["Controller"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Controller"].ToString(),
                        ActionName = ds.Tables[0].Rows[0]["ActionName"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["ActionName"].ToString(),
                        OrderNumber = ds.Tables[0].Rows[0]["OrderNumber"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["OrderNumber"]),
                        Icon = ds.Tables[0].Rows[0]["Icon"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[0].Rows[0]["Icon"]),
                        IsSubMenu = ds.Tables[0].Rows[0]["IsSubMenu"] == DBNull.Value ? null : Convert.ToInt32(ds.Tables[0].Rows[0]["IsSubMenu"]),
                        IsActive = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"]),
                    };
                }
                else
                {
                    return new MenuMO
                    {
                    };
                }
            }
            catch (Exception ex)
            {
                return new MenuMO
                {
                };
            }
        }
        public async Task<ListData> SubMenudataList(int draw, string search, int pageIndex, int pageSize, string sortCol, string sortDir, int userId, CommonFilter _filter)
        {
            ListData listData = new ListData();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Action", "GetListSubMenu"));
                parameters.Add(new SqlParameter("@PageIndex", pageIndex));
                parameters.Add(new SqlParameter("@PageSize", pageSize));
                parameters.Add(new SqlParameter("@groupId", _filter.groupId));
                parameters.Add(new SqlParameter("@url", _filter.url));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageHeaderMenu", parameters.ToArray());
                listData = await BindSubMenuData(ds, draw, sortCol, sortDir);
                return listData;
            }
            catch (Exception ex)
            {
                return new ListData();
            }
        }
        public async Task<ListData> BindSubMenuData(DataSet ds, int draw, string sortCol, string sortDir)
        {
            int totalRecord = 0;
            ListData listData = new ListData();
            List<MenuList> list = new List<MenuList>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataView dv = new DataView(ds.Tables[0]);
                    //dv.Sort = sortCol + " " + sortDir;
                    totalRecord = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRow"].ToString());
                    list = dv.ToTable().AsEnumerable().Select(s => new MenuList
                    {
                        SrNo = s["SrNo"] == DBNull.Value ? 0 : Convert.ToInt32(s["SrNo"]),
                        MenuId = s["MenuId"] == DBNull.Value ? 0 : Convert.ToInt32(s["MenuId"]),
                        HeaderMenu = s["HeaderMenu"] == DBNull.Value ? "" : Convert.ToString(s["HeaderMenu"]),
                        MenuName = s["Name"] == DBNull.Value ? "" : Convert.ToString(s["Name"]),
                        Controller = s["Controller"] == DBNull.Value ? "" : Convert.ToString(s["Controller"]),
                        ActionName = s["ActionName"] == DBNull.Value ? "" : Convert.ToString(s["ActionName"]),
                        MangalName = s["MangalName"] == DBNull.Value ? "" : Convert.ToString(s["MangalName"]),
                        OrderNumber = s["OrderNumber"] == DBNull.Value ? "" : Convert.ToString(s["OrderNumber"]),
                        Icon = s["Icon"] == DBNull.Value ? "" : Convert.ToString(s["Icon"]),
                        IsActive = s["IsActive"] == DBNull.Value ? "" : Convert.ToString(s["IsActive"]),
                        Action = s["Action"] == DBNull.Value ? "" : Convert.ToString(s["Action"]),
                    }).ToList();
                }
            }
            listData.recordsFiltered = totalRecord;
            listData.recordsTotal = totalRecord;
            listData.data = list;
            listData.draw = draw;
            return listData;
        }
        public async Task<result> BindDropDown_HeaderMenu()
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "BindDropDownHeaderMenu"));
                DataSet ds = await _iSql.ExecuteProcedure("SP_GetData", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = true,
                        data = ds.Tables[0].AsEnumerable().Select(x => new
                        {
                            id = Convert.ToInt32(x["Id"]),
                            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Convert.ToString(x["Name"]).ToLower()),
                        }).ToList()
                    };
                }
                else
                {
                    return new result
                    {
                        status = false,
                        message = "No data available",
                    };
                }
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong",
                };
            }

        }        
        #endregion

        #region Menu Permission
        public async Task<List<MenuItem>> GetMenuPermissiondataList(CommonFilter _filter)
        {
            List<MenuItem> listData = new List<MenuItem>();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Action", "getlist"));
                parameters.Add(new SqlParameter("@GroupId", _filter.option1));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageMenuPermission", parameters.ToArray());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        listData = ds.Tables[0].AsEnumerable().Select(s => new MenuItem
                        {
                            Id = s["Id"] == DBNull.Value ? 0 : Convert.ToInt32(s["Id"]),
                            ParentId = s["ParentId"] == DBNull.Value ? null : Convert.ToInt32(s["ParentId"]),
                            Name = s["Name"] == DBNull.Value ? "" : Convert.ToString(s["Name"]),
                            OrderNumber = s["OrderNumber"] == DBNull.Value ? 0 : Convert.ToInt32(s["OrderNumber"]),
                            isChecked = s["IsChecked"] == DBNull.Value ? false : Convert.ToBoolean(s["IsChecked"]),
                            ActionName = s["ActionName"] == DBNull.Value ? "" : Convert.ToString(s["ActionName"]),
                            ControllerName = s["Controller"] == DBNull.Value ? "" : Convert.ToString(s["Controller"]),
                            IconClass = s["Icon"] == DBNull.Value ? "" : Convert.ToString(s["Icon"]),
                            url = s["url"] == DBNull.Value ? "" : Convert.ToString(s["url"]),

                            // New permission properties
                            CanAdd = s["CanAdd"] != DBNull.Value && Convert.ToBoolean(s["CanAdd"]),
                            CanList = s["CanList"] != DBNull.Value && Convert.ToBoolean(s["CanList"]),
                            CanEdit = s["CanEdit"] != DBNull.Value && Convert.ToBoolean(s["CanEdit"]),
                            CanDelete = s["CanDelete"] != DBNull.Value && Convert.ToBoolean(s["CanDelete"]),
                            CanActiveDeactive = s["CanActiveDeactive"] != DBNull.Value && Convert.ToBoolean(s["CanActiveDeactive"])
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return listData;
            }
            return listData;
        }
        public async Task<result> saveMenuPermissions(SaveGroupPermissionModel model)
        {
            try
            {
                // Convert permission list to DataTable for SQL
                DataTable dt = new DataTable();
                dt.Columns.Add("MenuId", typeof(int));
                dt.Columns.Add("CanAdd", typeof(bool));
                dt.Columns.Add("CanList", typeof(bool));
                dt.Columns.Add("CanEdit", typeof(bool));
                dt.Columns.Add("CanDelete", typeof(bool));
                dt.Columns.Add("CanActiveDeactive", typeof(bool));

                foreach (var p in model.Permissions)
                {
                    dt.Rows.Add(
                        p.MenuId,
                        p.CanAdd,
                        p.CanList,
                        p.CanEdit,
                        p.CanDelete,
                        p.CanActiveDeactive
                    );
                }

                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Insert"),
                    new SqlParameter("@GroupId", model.GroupId),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? "")
                };

                // Permission table
                var permissionParam = new SqlParameter("@PermissionTable", dt)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.MenuPermissionType"   // ⬅ MUST MATCH SQL TYPE NAME
                };
                param.Add(permissionParam);

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageMenuPermission", param.ToArray());

                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                        message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
                    };
                }

                return new result
                {
                    status = false,
                    message = "Something went wrong"
                };
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong: " + ex.Message
                };
            }
        }
        #endregion

    }
}
