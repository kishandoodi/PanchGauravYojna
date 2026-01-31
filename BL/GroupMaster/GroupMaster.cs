using DL;
using MO.Common;
using MO.GroupMaster;
using MO.MenuMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GroupMaster
{
    public class GroupMaster :IGroupMaster
    {
        private readonly ISQLHelper _iSql;
        public GroupMaster(ISQLHelper iSql)
        {
            _iSql = iSql;
        }
        public async Task<result> SaveGroup(GroupMO model)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();

                if (model.Guid != null)
                    param.Add(new SqlParameter("@Action", "UpdateGroup"));
                else
                    param.Add(new SqlParameter("@Action", "InsertGroup"));

                param.Add(new SqlParameter("@Guid", model.Guid));
                param.Add(new SqlParameter("@Name", model.Name));
                param.Add(new SqlParameter("@Description", model.Description));
                param.Add(new SqlParameter("@IsActive", model.IsActive));
                param.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? string.Empty));

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGroup", param.ToArray());

                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                        message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
                    };
                }
                else
                {
                    return new result
                    {
                        status = false,
                        message = "Something went wrong"
                    };
                }
            }
            catch (Exception)
            {
                return new result
                {
                    status = false,
                    message = "Something went wrong"
                };
            }
        }
        public async Task<List<GroupMO>> GetAllGroup()
        {
            List<GroupMO> _list = new List<GroupMO>();
            try
            {
                
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@Action", "GetAllGroup"));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGroup", parameters.ToArray());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            _list.Add(new GroupMO
                            {
                                Guid = row["Guid"] != DBNull.Value ? row["Guid"].ToString() : string.Empty,
                                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                                Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : string.Empty,
                                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : string.Empty,
                                IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"])
                            });
                        }                        
                    }
                }
                return _list;
            }
            catch(Exception ex)
            {
                return _list;
            }
        }
        public async Task<result> ToggleActive(string guid)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter("@Action", "ToggleActive"),
                new SqlParameter("@Guid", guid)
            };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGroup", param.ToArray());

                return new result
                {
                    status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                    message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
                };
            }
            catch(Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "something wrong"
                };
            }
        }

        public async Task<result> Delete(string guid)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Delete"),
                    new SqlParameter("@Guid", guid)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGroup", param.ToArray());

                return new result
                {
                    status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                    message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
                };
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "something went wrong"
                };
            }
        }

        public async Task<GroupMO> GetGroupByGuid(string guid)
        {
            try
            {
                GroupMO targetMO = new GroupMO();
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetGroupByGuid"));
                param.Add(new SqlParameter("@Guid", guid));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGroup", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new GroupMO
                    {
                        Guid = ds.Tables[0].Rows[0]["Guid"].ToString(),
                        Id = ds.Tables[0].Rows[0]["Id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]),
                        Name = ds.Tables[0].Rows[0]["Name"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Name"].ToString(),
                        Description = ds.Tables[0].Rows[0]["Description"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Description"].ToString(),
                        IsActive = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"]),
                    };
                }
                else
                {
                    return new GroupMO
                    {
                    };
                }
            }
            catch(Exception ex)
            {
                return new GroupMO
                {
                };
            }
        }

        public async Task<result> BindDropDown_Group()
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "BindDropDownGroup"));
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
    }
}
