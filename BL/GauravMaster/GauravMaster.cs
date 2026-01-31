using DL;
using MO.Common;
using MO.GroupMaster;
using MO.Master;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GauravMaster
{
    public class GauravMaster : IGauravMaster
    {
        private readonly ISQLHelper _iSql;

        public GauravMaster(ISQLHelper iSql)
        {
            _iSql = iSql;
        }

        #region Gaurav
        //================ SAVE/UPDATE ==================
        public async Task<result> SaveGaurav(GauravMasterMO model)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.Guid))
                    param.Add(new SqlParameter("@Action", "UpdateGaurav"));
                else
                    param.Add(new SqlParameter("@Action", "InsertGaurav"));

                    param.Add(new SqlParameter("@Guid", (object?)model.Guid ?? DBNull.Value));
                    param.Add(new SqlParameter("@Name", model.Name));
                    param.Add(new SqlParameter("@MangalName", model.MangalName));                
                    param.Add(new SqlParameter("@IsActive", model.IsActive));
                    param.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? 0));
                    DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGaurav", param.ToArray());
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
        public async Task<List<GauravMasterMO>> GetAllGaurav()
        {
            List<GauravMasterMO> list = new List<GauravMasterMO>();

            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetAllGaurav"));

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGaurav", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new GauravMasterMO
                        {
                            GauravId = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : null,
                            Guid = row["Guid"]?.ToString(),
                            Name = row["Name"]?.ToString(),
                            MangalName = row["MangalName"]?.ToString(),
                            IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]),
                            CreatedDate = row["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["CreatedDate"]),
                            CreatedBy = row["CreadtedBy"] == DBNull.Value ? null : Convert.ToInt64(row["CreadtedBy"]),
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

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGaurav", param.ToArray());

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

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGaurav", param.ToArray());

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

        public async Task<GauravMasterMO> GetGauravByGuid(string guid)
        {
            try
            {
                GauravMasterMO targetMO = new GauravMasterMO();
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetGauravByGuid"));
                param.Add(new SqlParameter("@Guid", guid));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGaurav", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new GauravMasterMO
                    {
                        Guid = ds.Tables[0].Rows[0]["Guid"].ToString(),
                        GauravId = ds.Tables[0].Rows[0]["Id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]),
                        Name = ds.Tables[0].Rows[0]["Name"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["Name"].ToString(),
                        MangalName = ds.Tables[0].Rows[0]["MangalName"] == DBNull.Value ? null : ds.Tables[0].Rows[0]["MangalName"].ToString(),                       
                        IsActive = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"]),
                    };
                }
                else
                {
                    return new GauravMasterMO
                    {
                    };
                }
            }
            catch (Exception ex)
            {
                return new GauravMasterMO
                {
                };
            }
        }
        #endregion

        #region Gaurav Department
        //================ SAVE/UPDATE ==================
        public async Task<result> SaveGauravDepartment(GauravDepartmentMO model)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.Guid))
                    param.Add(new SqlParameter("@Action", "UpdateGauravDepartment"));
                else
                    param.Add(new SqlParameter("@Action", "InsertGauravDepartment"));

                param.Add(new SqlParameter("@Guid", (object?)model.Guid ?? DBNull.Value));
                param.Add(new SqlParameter("@GauravId", model.GauravId));
                param.Add(new SqlParameter("@DepartmentId", model.DepartmentId));
                param.Add(new SqlParameter("@IsActive", model.IsActive));
                param.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? 0));
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDepartment", param.ToArray());
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
        public async Task<List<GauravDepartmentMO>> GetAllGauravDepartment()
        {
            List<GauravDepartmentMO> list = new List<GauravDepartmentMO>();

            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetAllGauravDepartment"));

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDepartment", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new GauravDepartmentMO
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Guid = Convert.ToString(row["Guid"]),
                            Gaurav = row["GauravName"] == DBNull.Value ? null : Convert.ToString(row["GauravName"]),
                            Department = row["DepartmentName"] == DBNull.Value ? null : Convert.ToString(row["DepartmentName"]),
                            GauravId = row["GauravId"] == DBNull.Value ? 0 : Convert.ToInt32(row["GauravId"]),
                            DepartmentId = row["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DepartmentId"]),
                            IsActive = row["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(row["IsActive"]),
                            CreatedDate = row["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["CreatedDate"]),
                            CreatedBy = row["CreatedBy"] == DBNull.Value ? null : Convert.ToInt64(row["CreatedBy"]),
                            UpdatedDate = row["UpdatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["UpdatedDate"]),
                            UpdatedBy = row["UpdatedBy"] == DBNull.Value ? null : Convert.ToString(row["UpdatedBy"]),
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

        public async Task<result> GauravDepartmentToggleActive(string guid)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter("@Action", "GauravDepartmentToggleActive"),
                new SqlParameter("@Guid", guid)
            };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDepartment", param.ToArray());

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

        public async Task<result> GauravDepartmentDelete(string guid)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Delete"),
                    new SqlParameter("@Guid", guid)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDepartment", param.ToArray());

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
        #endregion

        #region Gaurav for Dashboard
        public async Task<List<GauravMasterMO>> GetAllGauravDashboard(int districtId, int FyId)
        {
            List<GauravMasterMO> list = new List<GauravMasterMO>();

            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "GetAllGauravDashboard"));
                param.Add(new SqlParameter("@DistrictId", districtId));
                param.Add(new SqlParameter("@FinancialYearId", FyId));

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGaurav_Dashboard", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new GauravMasterMO
                        {
                            GauravId = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : null,
                            Guid = row["Guid"]?.ToString(),
                            Name = row["Name"]?.ToString(),
                            MangalName = row["MangalName"]?.ToString(),
                            IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]),
                            CreatedDate = row["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["CreatedDate"]),
                            CreatedBy = row["CreadtedBy"] == DBNull.Value ? null : Convert.ToInt64(row["CreadtedBy"]),
                            UpdatedDate = row["UpdatedDate"] == DBNull.Value ? null : Convert.ToDateTime(row["UpdatedDate"]),
                            UpdatedBy = row["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt64(row["UpdatedBy"]),
                            StatusId = row["StatusId"] != DBNull.Value ? Convert.ToInt32(row["StatusId"]) : null,
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
        #endregion
    }
}
