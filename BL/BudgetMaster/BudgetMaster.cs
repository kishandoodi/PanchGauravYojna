using DL;
using MO.BudgetMaster;
using MO.Common;
using MO.GauravProfile;
using MO.ProfileUser;
using MO.WebsiteMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        public async Task<result> GetVettingList(int garauvId, int districtId, int FyId)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
        {
                    new SqlParameter("@Action", "GetVettingList"),
            new SqlParameter("@DistrictId", districtId),
            new SqlParameter("@GauravGuid", garauvId),
            new SqlParameter("@FinancialYearId", FyId)
        };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = ds.Tables[0].AsEnumerable().Select(row =>
                    {
                        VettingList obj = new VettingList
                        {
                            rowId = row["RowId"] != DBNull.Value ? Convert.ToInt32(row["RowId"]) : 0,
                            SubQuestionMasterId = row["SubQuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["SubQuestionMasterId"]) : 0,
                            AnswerValue = row["AnswerValue"]?.ToString() ?? "",
                            QuestionMasterId = row["QuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["QuestionMasterId"]) : 0,
                            GauravId = row["GauravId"] != DBNull.Value ? Convert.ToInt64(row["GauravId"]) : 0,
                            DistrictId = row["DistrictId"] != DBNull.Value ? Convert.ToInt32(row["DistrictId"]) : 0
                        };

                        if (obj.SubQuestionMasterId == 17)
                        {
                            int enumId = 0;
                            int.TryParse(obj.AnswerValue, out enumId);

                            EnumProfileQuestion enumValue =
                                Enum.IsDefined(typeof(EnumProfileQuestion), enumId)
                                ? (EnumProfileQuestion)enumId
                                : EnumProfileQuestion.OtherActivity;

                            obj.AnswerValue = GetDisplayName(enumValue);
                        }

                        return obj;
                    }).ToList();

                    _result.status = true;
                    _result.message = "Answers fetched successfully.";
                    _result.data = list;

                    var grouped = list
                    .GroupBy(x => new { x.rowId, x.GauravId, x.DistrictId })
                    .Select(g => new VettingRowVM
                       {
                 RowId = (int)g.Key.rowId,
                 GauravId = (int)g.Key.GauravId,
                 DistrictId = (int)g.Key.DistrictId,

                QuestionMasterId = g.First().QuestionMasterId,
                 SubQuestionMasterId = g.First().SubQuestionMasterId,
                 AnswerValue = g.First().AnswerValue,
                  Fieldtype = g.First().Fieldtype,

                    Columns = g
                     .GroupBy(x => x.SubQuestionMasterId)
                      .ToDictionary(
                          x => x.Key,
                             x => x.First().AnswerValue ?? ""
                              )
                          })
                                .ToList();
                    _result.status = true;
                    _result.message = "Answers fetched successfully.";
                    _result.data = grouped ?? new List<VettingRowVM>();
                }
                else
                {
                    _result.status = false;
                    _result.message = "No answers found.";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        public static string GetDisplayName(Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName()
                ?? enumValue.ToString();
        }
        public async Task<result> GetPendingVettingList(int RawId, int garauvId, int districtId, int FyId, int SubQuestionMasterId, int QuestionMasterId)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
        {
                    new SqlParameter("@Action", "GetPendingVettingList"),
                    new SqlParameter("@RawId", RawId),
            new SqlParameter("@DistrictId", districtId),
            new SqlParameter("@GauravGuid", garauvId),
            new SqlParameter("@FinancialYearId", FyId),
            //new SqlParameter("@SubQuestionMasterId", SubQuestionMasterId),
            //new SqlParameter("@QuestionMasterId", QuestionMasterId)

        };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = ds.Tables[0].AsEnumerable().Select(row =>
                    {
                        VettingList obj = new VettingList
                        {
                            rowId = row["RowId"] != DBNull.Value ? Convert.ToInt32(row["RowId"]) : 0,
                            SubQuestionMasterId = row["SubQuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["SubQuestionMasterId"]) : 0,
                            AnswerValue = row["AnswerValue"]?.ToString() ?? "",
                            QuestionMasterId = row["QuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["QuestionMasterId"]) : 0,
                            GauravId = row["GauravId"] != DBNull.Value ? Convert.ToInt32(row["GauravId"]) : 0,
                            Fieldtype = row["Fieldtype"]?.ToString() ?? ""

                        };

                        if (obj.SubQuestionMasterId == 17)
                        {
                            int enumId = 0;
                            int.TryParse(obj.AnswerValue, out enumId);

                            EnumProfileQuestion enumValue =
                                Enum.IsDefined(typeof(EnumProfileQuestion), enumId)
                                ? (EnumProfileQuestion)enumId
                                : EnumProfileQuestion.OtherActivity;

                            obj.AnswerValue = GetDisplayName(enumValue);
                        }

                        return obj;
                    }).ToList();

                    //_result.status = true;
                    //_result.message = "Answers fetched successfully.";
                    //_result.data = list;
                    var grouped = list
                      .GroupBy(x => x.rowId)
                       .Select(g => new VettingRowVM
                        {
                      RowId = (int)g.Key,

                         // row level info
                            QuestionMasterId = g.First().QuestionMasterId,
                             SubQuestionMasterId = g.First().SubQuestionMasterId,
                             AnswerValue = g.First().AnswerValue,
                                   Fieldtype = g.First().Fieldtype,

                        // table columns
                            Columns = g
                              .GroupBy(x => x.SubQuestionMasterId)
                                                    .ToDictionary(
                             x => x.Key,
                                            x => x.First().AnswerValue ?? ""
                                      )
                                     })
                               .ToList();




                    _result.status = true;
                    _result.message = "Answers fetched successfully.";
                    _result.data = grouped ?? new List<VettingRowVM>();
                }
                else
                {
                    _result.status = false;
                    _result.message = "No answers found.";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        public async Task<result> SaveVettingData(VettingSaveVM obj)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
                    {
                    new SqlParameter("@Action", "SavePendingVetted"),
                       new SqlParameter("@RawId", obj.RowId),
                        new SqlParameter("@ActivityName", obj.ActivityName),
                        new SqlParameter("@Budget", obj.Budget),
                        new SqlParameter("@Activity", obj.Activity),
                        new SqlParameter("@DistrictId", obj.DistrictId),
                        new SqlParameter("@GauravGuid", obj.GauravId),
                        new SqlParameter("@SubQuestionMasterId", obj.SubQuestionMasterId),
                        new SqlParameter("@QuestionMasterId", obj.QuestionMasterId),
                        new SqlParameter("@FinancialYearId", obj.FinancialYear_Id)
                    };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param.ToArray());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    _result.status = true;
                    _result.message = "Saved successfully";
                }
                else 
                {
                    _result.status = false;
                    _result.message = "Somthing Error";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Enter Valid Value In Budget ";
            }

            return _result;
        }
        public async Task<result> GetPendingVettingList(VettingSaveVM obj)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
                    {
                    new SqlParameter("@Action", "GetPendingVettedList"),
                      new SqlParameter("@RawId", obj.RowId),
                    new SqlParameter("@DistrictId", obj.DistrictId),
                        new SqlParameter("@GauravGuid", obj.GauravId),
                        new SqlParameter("@SubQuestionMasterId", obj.SubQuestionMasterId),
                        new SqlParameter("@QuestionMasterId", obj.QuestionMasterId)
                    };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = ds.Tables[0].AsEnumerable().Select(row =>
                    {
                        VettingSaveVM obj = new VettingSaveVM
                        {
                           // RowId = row["RowId"] != DBNull.Value ? Convert.ToInt32(row["RowId"]) : 0,
                            Activity = row["Activity"]?.ToString() ?? "",
                            ActivityName = row["ActivityName"]?.ToString() ?? "",
                            Budget = row["Budget"]?.ToString() ?? "",
                        };

                        return obj;
                    }).ToList();

                    _result.status = true;
                    _result.message = "Answers fetched successfully.";
                    _result.data = list;
                }
                else
                {
                    _result.status = false;
                    _result.message = "Somthing Error";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = ex.Message;
            }

            return _result;
        }
    }
}
