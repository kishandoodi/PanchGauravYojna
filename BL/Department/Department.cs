using BL.Department;
using DL;
using MO.Common;
using MO.GroupMaster;
using MO.Master;
using System.Data;
using System.Data.SqlClient;

public class Department : IDepartment
{
    private readonly ISQLHelper _iSql;

    public Department(ISQLHelper iSql)
    {
        _iSql = iSql;
    }

    //================ SAVE/UPDATE ==================
    public async Task<result> SaveDepartment(DepartmentMO model)
    {
        try
        {
            List<SqlParameter> param = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(model.Guid))
                param.Add(new SqlParameter("@Action", "UpdateDepartment"));
            else
                param.Add(new SqlParameter("@Action", "InsertDepartment"));

            param.Add(new SqlParameter("@Guid", (object?)model.Guid ?? DBNull.Value));
            param.Add(new SqlParameter("@Name", model.Name));
            param.Add(new SqlParameter("@IsActive", model.IsActive));
            param.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? 0));

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageDepartment", param.ToArray());

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
                return new result { status = false, message = "Something went wrong" };
            }
        }
        catch (Exception)
        {
            return new result { status = false, message = "Something went wrong" };
        }
    }

    //================ GET LIST ==================
    public async Task<List<DepartmentMO>> GetAllDepartment()
    {
        List<DepartmentMO> list = new List<DepartmentMO>();

        try
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Action", "GetAllDepartment"));

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageDepartment", param.ToArray());

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new DepartmentMO
                    {
                        DepartmentId = row["DepartmentId"] != DBNull.Value ? Convert.ToInt32(row["DepartmentId"]) : null,
                        Guid = row["Guid"]?.ToString(),
                        Name = row["Name"]?.ToString(),
                        IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]),
                        CreatedDate = row["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["CreatedDate"]),
                        CreatedBy = row["CreatedBy"] == DBNull.Value ? null : Convert.ToInt64(row["CreatedBy"]),
                        UpdatedDate = row["UpdatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["UpdatedDate"]),
                        UpdatedBy = row["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt64(row["UpdatedBy"]),
                    });
                }
            }

            return list;
        }
        catch (Exception ex)
        {
            return list;
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

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageDepartment", param.ToArray());

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

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageDepartment", param.ToArray());

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
            param.Add(new SqlParameter("@Action", "GetDepartmentByGuid"));
            param.Add(new SqlParameter("@Guid", guid));
            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageDepartment", param.ToArray());
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
        catch (Exception ex)
        {
            return new GroupMO
            {
            };
        }
    }
}
