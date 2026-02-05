using DL;
using MO.BudgetMaster;
using MO.Common;
using MO.WebsiteMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BudgetMaster
{
    public class BudgetMaster : IBudgetMaster
    {
        #region Properties
        private readonly ISQLHelper _iSql;
        #endregion
        #region Constructor
        public BudgetMaster(ISQLHelper iSql)
        {
            _iSql = iSql;
        }
        #endregion

        public async Task<result> BindDistrictDropDownVetting()
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "BindDistrictDropDownAll"));
                DataSet ds = await _iSql.ExecuteProcedure("SP_GetData", param.ToArray());
                if (ds.Tables.Count > 0)
                {
                    return new result
                    {
                        status = true,
                        data = ds.Tables[0].AsEnumerable().Select(x => new
                        {
                            Id = Convert.ToInt32(x["Id"]),
                            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Convert.ToString(x["Name"]).ToLower()),
                        }).ToList()
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
        public async Task<result> BindGauravDropDownVetting(string userId)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Action", "BindGauravDropDownVetting"));
                param.Add(new SqlParameter("@Id", userId));
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

        public async Task<List<VettingList>> GetVettingList(int garauvId, int districtId, int FyId)
        {
            try
            {
                List<VettingList> list = new List<VettingList>();

                var param = new SqlParameter[]
                {
        new SqlParameter("@Action", "GetVettingList"),
        new SqlParameter("@GauravGuid", garauvId),
        new SqlParameter("@DistrictId", districtId),
        new SqlParameter("@FinancialYearId", FyId)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new VettingList
                        {
                            Id = row.Field<int>("RowId"),
                            Gatividhi = row.Field<string>("Gatividhi"),
                            GatividhiName = row.Field<string>("GatividhiName"),
                            Budget = row.Field<string>("Budget")
                        });
                    }
                }

                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }


    }
}
