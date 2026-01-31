using BL.Common;
using DL;
using MO.Common;
using MO.ProfileUser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ProfileUser
{
    public class ProfileUser : IProfileUser
    {
        #region Properties
        private readonly ISQLHelper _iSql;
        private readonly ICommon _iCommon;
        #endregion
        #region Constructor
        public ProfileUser(ISQLHelper iSql, ICommon iCommon)
        {
            _iSql = iSql;
            _iCommon = iCommon;

        }
        #endregion
        public async Task<result> SaveUserAsync(UserMO model)
        {
            try
            {
                string type = "";
                if (model.Guid != null)
                {
                    type = "Update";
                }
                else
                {
                    type = "Insert";
                }
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action",type),
                    new SqlParameter("@Guid", model.Guid ?? Guid.NewGuid().ToString()),
                    new SqlParameter("@Name", model.Name),
                    new SqlParameter("@DisplayName", model.DisplayName),
                    new SqlParameter("@SSOID", model.SSOID),
                    new SqlParameter("@DistrictId", model.DistrictId),
                    new SqlParameter("@DepartmentId", model.DepartmentId),
                    new SqlParameter("@RoleId", model.RoleId),
                    new SqlParameter("@UserLevel", model.UserLevel),
                    new SqlParameter("@Designation", model.Designation),
                    new SqlParameter("@Mobile", model.Mobile),
                    new SqlParameter("@WhatsappMobile", model.WhatsappMobile),
                    new SqlParameter("@EmailId", model.EmailId),
                    new SqlParameter("@CreatedBy", ""), // Or Logged-in User
                    new SqlParameter("@GroupId", model.GroupId), // Or Logged-in User
                    //new SqlParameter("@UserLevel", model.UserLevel), // Or Logged-in User
                    new SqlParameter("@IsActive", 1)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageUser", param.ToArray());

                return new result
                {
                    status = true,
                    message = ds.Tables[0].Rows[0]["message"].ToString()
                };
            }
            catch (Exception ex)
            {
                return new result
                {
                    status = false,
                    message = "Error while saving user: " + ex.Message
                };
            }
        }
        public async Task<List<UserListMO>> GetUserListAsync()
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "GetList")
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageUser", param.ToArray());

                var userList = new List<UserListMO>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        userList.Add(new UserListMO
                        {
                            Guid = row["Guid"] != DBNull.Value ? row["Guid"].ToString() : null,
                            Name = row["Name"]?.ToString(),
                            DisplayName = row["DisplayName"]?.ToString(),
                            SSOID = row["SSOID"]?.ToString(),
                            DistrictName = row["DistrictName"]?.ToString(),
                            DepartmentName = row["DepartmentName"]?.ToString(),
                            RoleName = row["RoleName"]?.ToString(),
                            UserLevel = row["UserLevel"]?.ToString(),
                            Designation = row["Designation"]?.ToString(),
                            Mobile = row["Mobile"]?.ToString(),
                            WhatsappMobile = row["WhatsappMobile"]?.ToString(),
                            EmailId = row["EmailId"]?.ToString(),
                            Group = row["GroupName"]?.ToString(),
                            IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }

                return userList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching user list: " + ex.Message);
            }
        }
        public async Task<UserMO> GetUserByGuidAsync(string guid)
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "GetByGuid"),
                    new SqlParameter("@Guid", guid)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageUser", param.ToArray());

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    var user = new UserMO
                    {
                        Guid = row["Guid"] != DBNull.Value ? row["Guid"].ToString() : null,
                        Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : null,
                        DisplayName = row["DisplayName"] != DBNull.Value ? row["DisplayName"].ToString() : null,
                        SSOID = row["SSOID"] != DBNull.Value ? row["SSOID"].ToString() : null,
                        DistrictId = Convert.ToInt32(row["DistrictId"]),
                        DepartmentId = Convert.ToInt32(row["DepartmentId"]),
                        GroupId = Convert.ToInt32(row["GroupId"]),
                        UserLevel = Convert.ToInt32(row["UserLevelId"]),
                        Designation = row["Designation"] != DBNull.Value ? row["Designation"].ToString() : null,
                        Mobile = row["Mobile"] != DBNull.Value ? row["Mobile"].ToString() : null,
                        WhatsappMobile = row["WhatsappMobile"] != DBNull.Value ? row["WhatsappMobile"].ToString() : null,
                        EmailId = row["EmailId"] != DBNull.Value ? row["EmailId"].ToString() : null,
                        IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : false
                    };

                    // Populate dropdown lists if needed
                    user.DistrictList = await _iCommon.DDL_DistrictAsync();
                    user.DepartmentList = await _iCommon.DDL_DepartmentAsync();
                    user.GroupList = await _iCommon.DDL_GroupAsync();
                    //user.RoleList = await _iCommon.DDL_RoleAsync();
                    user.UserLevelList = await _iCommon.DDL_UserLevelAsync();

                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching user details: " + ex.Message);
            }
        }
        public async Task<result> ActiveDeactiveUserAsync(string guid, string modifiedBy)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "IsActiveUser"),
                    new SqlParameter("@Guid", guid ),
                    new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = (object)modifiedBy ?? DBNull.Value }
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageUser", parameters.ToArray());

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    bool status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"]) == 1;
                    string message = ds.Tables[0].Rows[0]["Message"].ToString();
                    return new result { status = status, message = message };
                }

                return new result { status = false, message = "No response from database." };
            }
            catch (Exception ex)
            {
                return new result { status = false, message = "Error: " + ex.Message };
            }
        }
        public async Task<result> Delete(string guid,string modifiedBy)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Delete"),
                    new SqlParameter("@Guid", guid),
                    new SqlParameter("@CreatedBy", modifiedBy)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageUser", param.ToArray());

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
    }
}
