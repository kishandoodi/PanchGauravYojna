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

namespace BL.FinancialYear
{
    public class FinancialYear : IFinancialYear
    {
        private readonly ISQLHelper _iSql;
        public FinancialYear(ISQLHelper iSql)
        {
            _iSql = iSql;
        }
        public async Task<result> SaveFinancialYear(FinancialYearCM model)
        {
            List<SqlParameter> param = new();

            param.Add(new SqlParameter("@Action",
                string.IsNullOrEmpty(model.Guid) ? "Insert" : "Update"));

            param.Add(new SqlParameter("@Guid", (object?)model.Guid ?? DBNull.Value));
            param.Add(new SqlParameter("@Code", model.Year));
            param.Add(new SqlParameter("@Year", model.Year));
            param.Add(new SqlParameter("@FinancialYear", model.FinancialYear));
            param.Add(new SqlParameter("@IsCurrent", model.IsCurrent));
            param.Add(new SqlParameter("@Status", model.IsActive));
            param.Add(new SqlParameter("@CreatedBy", model.CreatedBy ?? 0));

            DataSet ds = await _iSql.ExecuteProcedure("SP_MaganeFinancialYear", param.ToArray());

            return new result
            {
                status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
            };
        }
        public async Task<List<FinancialYearCM>> GetAllFinancialYear()
        {
            List<FinancialYearCM> list = new();

            List<SqlParameter> param = new()
            {
                new SqlParameter("@Action", "GetAll")
            };

            DataSet ds = await _iSql.ExecuteProcedure("SP_MaganeFinancialYear", param.ToArray());

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new FinancialYearCM
                {
                    FinancialYear_Id = Convert.ToInt32(row["FinancialYear_Id"]),
                    Guid = row["Guid"].ToString(),
                    Code = row["Code"].ToString(),
                    Year = row["Year"].ToString(),
                    FinancialYear = row["FinancialYear"].ToString(),
                    IsCurrent = Convert.ToBoolean(row["IsCurrent"]),
                    IsActive = Convert.ToBoolean(row["Status"]),
                    CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                });
            }

            return list;
        }
        public async Task<result> ToggleActive(string guid)
        {
            return await ExecuteSimpleAction("ToggleActive", guid);
        }
        public async Task<result> IsCurrent(string guid)
        {
            return await ExecuteSimpleAction("SetCurrent", guid);
        }
        public async Task<result> Delete(string guid)
        {
            return await ExecuteSimpleAction("Delete", guid);
        }
        private async Task<result> ExecuteSimpleAction(string action, string guid)
        {
            List<SqlParameter> param = new()
    {
        new SqlParameter("@Action", action),
        new SqlParameter("@Guid", guid)
    };

            DataSet ds = await _iSql.ExecuteProcedure("SP_MaganeFinancialYear", param.ToArray());
            try
            {

            }
            catch(Exception ex)
            {

            }
            return new result
            {
                status = Convert.ToBoolean(ds.Tables[0].Rows[0]["Status"]),
                message = Convert.ToString(ds.Tables[0].Rows[0]["Message"])
            };
        }
    }
}
