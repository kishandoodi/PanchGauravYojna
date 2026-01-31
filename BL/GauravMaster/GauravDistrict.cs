using DL;
using MO.Common;
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
    public class GauravDistrict : IGauravDistrict
    {
        private readonly ISQLHelper _iSql;

        public GauravDistrict(ISQLHelper iSql)
        {
            _iSql = iSql;
        }

        //================ SAVE/UPDATE ==================
        public async Task<result> Save(GauravDistrictMO model)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(model.Guid))
                    param.Add(new SqlParameter("@Action", "Update"));
                else
                    param.Add(new SqlParameter("@Action", "Insert"));

                param.Add(new SqlParameter("@Guid", (object?)model.Guid ?? DBNull.Value));
                param.Add(new SqlParameter("@DistrictId", model.DistrictId ?? (object)DBNull.Value));
                param.Add(new SqlParameter("@GauraveId", model.GauraveId ?? (object)DBNull.Value));
                param.Add(new SqlParameter("@FinancialYearId", model.FyId ?? (object)DBNull.Value));
                param.Add(new SqlParameter("@GauravName", model.GauravName ?? ""));
                param.Add(new SqlParameter("@MangalName", model.MangalName ?? ""));
                param.Add(new SqlParameter("@Vivran", model.Vivran ?? ""));
                param.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? "system"));

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDistrict", param.ToArray());

                return new result
                {
                    status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                    message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
                };
            }
            catch
            {
                return new result { status = false, message = "Something went wrong" };
            }
        }

        //================ LIST ==================
        public async Task<List<GauravDistrictMO>> GetList(int FyId)
        {
            List<GauravDistrictMO> list = new List<GauravDistrictMO>();

            List<SqlParameter> param = new List<SqlParameter>
        {
            new SqlParameter("@Action", "List"),
            new SqlParameter("@FinancialYearId", FyId)
        };

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDistrict", param.ToArray());

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new GauravDistrictMO
                    {
                        GauravDistId = Convert.ToInt64(row["GauravDist_Id"]),
                        Guid = row["Guid"].ToString(),
                        DistrictId = row["District_Id"] != DBNull.Value ? Convert.ToInt32(row["District_Id"]) : null,
                        GauraveId = row["Gaurave_Id"] != DBNull.Value ? Convert.ToInt32(row["Gaurave_Id"]) : null,
                        GauravName = row["GauravName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        MangalName = row["MangalName"].ToString(),
                        Vivran = row["Vivran"].ToString(),                        
                    });
                }
            }

            return list;
        }

        //================== DELETE ==================
        public async Task<result> Delete(string guid, int FyId)
        {
            List<SqlParameter> param = new List<SqlParameter>
        {
            new SqlParameter("@Action", "Delete"),
            new SqlParameter("@Guid", guid),
            new SqlParameter("@FinancialYearId", FyId)
        };

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDistrict", param.ToArray());

            return new result
            {
                status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
            };
        }

        //================== TOGGLE ACTIVE ==================
        public async Task<result> ToggleActive(string guid, int FyId)
        {
            List<SqlParameter> param = new List<SqlParameter>
        {
            new SqlParameter("@Action", "IsActive"),
            new SqlParameter("@Guid", guid),
            new SqlParameter("@FinancialYearId", FyId)
        };

            DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravDistrict", param.ToArray());

            return new result
            {
                status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
            };
        }
    }

}
