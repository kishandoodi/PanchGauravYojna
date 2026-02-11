using DL;
using Microsoft.AspNetCore.Mvc;
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
                    var fullList = ds.Tables[0].AsEnumerable().Select(row =>
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

                    // 🔹 Filter only SubQuestionMasterId = 17 for main rows
                    var filteredList = fullList
                        .Where(x => x.SubQuestionMasterId == 17)
                        .ToList();

                    var grouped = filteredList
                        .GroupBy(x => new { x.rowId, x.GauravId, x.DistrictId })
                        .Select(g => new VettingRowVM
                        {
                            RowId = (int)g.Key.rowId,
                            GauravId = (int)g.Key.GauravId,
                            DistrictId = (int)g.Key.DistrictId,

                            QuestionMasterId = g.First().QuestionMasterId,
                            SubQuestionMasterId = 17,
                            AnswerValue = g.First().AnswerValue,
                            Fieldtype = g.First().Fieldtype,

                            // 🔹 Columns from FULL list
                            Columns = fullList
                                .Where(x => x.rowId == g.Key.rowId
                                         && x.GauravId == g.Key.GauravId
                                         && x.DistrictId == g.Key.DistrictId)
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
                           RowId = row["RawId"] != DBNull.Value ? Convert.ToInt32(row["RawId"]) : 0,
                            Activity = row["Activity"]?.ToString() ?? "",
                            ActivityName = row["ActivityName"]?.ToString() ?? "",
                            Budget = row["Budget"]?.ToString() ?? "",
                            SubQuestionMasterId = row["SubQuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["SubQuestionMasterId"]) : 0,
                            QuestionMasterId = row["QuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["QuestionMasterId"]) : 0,
                            GauravId = row["GauravId"] != DBNull.Value ? Convert.ToInt32(row["GauravId"]) : 0,
                            DistrictId = row["DistrictId"] != DBNull.Value ? Convert.ToInt32(row["DistrictId"]) : 0,
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

        public async Task<result> DeleteVettedList(int RawId, int garauvId, int DistrictId, int SubQuestionMasterId, int QuestionMasterId)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
                    {
                    new SqlParameter("@Action", "DeleteVettedList"),
                       new SqlParameter("@RawId", RawId),
                        new SqlParameter("@DistrictId", DistrictId),
                        new SqlParameter("@GauravGuid", garauvId),
                        new SqlParameter("@SubQuestionMasterId", SubQuestionMasterId),
                        new SqlParameter("@QuestionMasterId", QuestionMasterId),
                    };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param.ToArray());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    _result.status = true;
                    _result.message = "Record Deleted successfully";
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
                _result.message = "Somthing Error";
            }

            return _result;
        }
        public async Task<result> UpdateVettedList(VettingSaveVM obj)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
                    {
                    new SqlParameter("@Action", "UpdateVettedList"),
                       new SqlParameter("@RawId", obj.RowId),
                        new SqlParameter("@DistrictId", obj.DistrictId),
                        new SqlParameter("@GauravGuid", obj.GauravId),
                        new SqlParameter("@SubQuestionMasterId", obj.SubQuestionMasterId),
                        new SqlParameter("@QuestionMasterId", obj.QuestionMasterId),
                        new SqlParameter("@Budget", obj.Budget),
                    };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Budget", param.ToArray());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    _result.status = true;
                    _result.message = "Record Updated successfully";
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
                _result.message = "Somthing Error";
            }

            return _result;
        }
        public async Task<result> GetStep2Questions(string gauravGuid)
        {
            result _result = new result();
            List<QuestionModel_step2> list = new List<QuestionModel_step2>();

            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@GauravGuid", gauravGuid)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GetQuestions_step2", param.ToArray());

                if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                {
                    _result.status = false;
                    _result.message = "No questions found.";
                    return _result;
                }

                // --------------------------
                // 1️⃣ MAP QUESTIONS
                // --------------------------
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new QuestionModel_step2
                    {
                        QuestionMasterId = Convert.ToInt64(dr["QuestionMasterId"]),
                        DisplayNumber = dr["DisplayNumber"].ToString(),
                        QuestionText = dr["QuestionText"].ToString(),
                        SubQuestions = new List<SubQuestionModel_Step2>()
                    });
                }

                // --------------------------
                // 2️⃣ MAP SUB QUESTIONS
                // --------------------------
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        var sub = new SubQuestionModel_Step2
                        {
                            SubQuestionMasterId = Convert.ToInt64(dr["SubQuestionMasterId"]),
                            DisplayNumber = dr["DisplayNumber"]?.ToString(),
                            OrderNumber = Convert.ToInt32(dr["OrderNumber"]),
                            QuestionText = dr["QuestionText"]?.ToString(),
                            Fieldtype = dr["Fieldtype"]?.ToString(),
                            QuestionType = Convert.ToInt32(dr["QuestionType"])
                        };

                        // ❗ सभी questions में same QuestionType की sub-questions लगाएँ
                        foreach (var q in list)
                        {
                            if (q.QuestionType == sub.QuestionType || q.SubQuestions.Count >= 0)
                            {
                                q.SubQuestions.Add(sub);
                            }
                        }
                    }
                }

                // SUCCESS
                _result.status = true;
                _result.message = "Step-2 questions loaded successfully.";
                _result.data = list;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        public async Task<result> GetVettedQuestions(string GauravId)
        {
            result _result = new result();
            List<QuestionModel_VettedQuestions> list = new List<QuestionModel_VettedQuestions>();

            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@GauravId", GauravId)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GetQuestions_Vetted", param.ToArray());

                if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                {
                    _result.status = false;
                    _result.message = "No questions found.";
                    return _result;
                }

                // --------------------------
                // 1️⃣ MAP QUESTIONS
                // --------------------------
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new QuestionModel_VettedQuestions
                    {
                        QuestionMasterId = Convert.ToInt64(dr["QuestionMasterId"]),
                        DisplayNumber = dr["DisplayNumber"].ToString(),
                        QuestionText = dr["QuestionText"].ToString(),
                        SubQuestions = new List<SubQuestionModel_VettedQuestions>()
                    });
                }

                // --------------------------
                // 2️⃣ MAP SUB QUESTIONS
                // --------------------------
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        var sub = new SubQuestionModel_VettedQuestions
                        {
                            SubQuestionMasterId = Convert.ToInt64(dr["SubQuestionMasterId"]),
                            DisplayNumber = dr["DisplayNumber"]?.ToString(),
                            OrderNumber = Convert.ToInt32(dr["OrderNumber"]),
                            QuestionText = dr["QuestionText"]?.ToString(),
                            Fieldtype = dr["Fieldtype"]?.ToString(),
                            QuestionType = Convert.ToInt32(dr["QuestionType"])
                        };

                        // ❗ सभी questions में same QuestionType की sub-questions लगाएँ
                        foreach (var q in list)
                        {
                            if (q.QuestionType == sub.QuestionType || q.SubQuestions.Count >= 0)
                            {
                                q.SubQuestions.Add(sub);
                            }
                        }
                    }
                }

                // SUCCESS
                _result.status = true;
                _result.message = "Step-2 questions loaded successfully.";
                _result.data = list;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }



    }

}
