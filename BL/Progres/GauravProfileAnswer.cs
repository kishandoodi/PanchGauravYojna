using DL;
using MO.Common;
using MO.GauravProfile;
using MO.Indicator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL.Progres
{
    public class GauravProfileAnswer : IGauravProfileAnswer
    {
        #region Properties
        private readonly ISQLHelper _iSql;
        #endregion
        #region Constructor
        public GauravProfileAnswer(ISQLHelper iSql)
        {
            _iSql = iSql;
        }
        #endregion

        public async Task<result> SaveAnswersUsingProcedure(List<AnswerBlock> answers, string gauravId, int gauravDistrictId, int districtId, string createdBy)
        {
            result _result = new result();
            try
            {
                foreach (var block in answers)
                {
                    if (!string.IsNullOrEmpty(block.questionId))
                    {
                        foreach (var ans in block.answers)
                        {
                            if (!string.IsNullOrEmpty(block.questionId))
                            {
                                var parameters = new List<SqlParameter>
                                {
                                    new SqlParameter("@Action", "SubAnswer"),
                                    new SqlParameter("@QuestionId", block.questionId),
                                    new SqlParameter("@SubQuestionId", DBNull.Value),
                                    new SqlParameter("@Answer", ans.answer ?? ""),
                                    new SqlParameter("@GauravGuid", gauravId),
                                    new SqlParameter("@GauravDistrictId", gauravDistrictId),
                                    new SqlParameter("@DistrictId", districtId),
                                    new SqlParameter("@rowId", ans.rowId),
                                    new SqlParameter("@CreatedBy", createdBy)
                                };
                                await _iSql.ExecuteProcedure("SP_SaveGauravAnswers", parameters.ToArray());
                            }
                            else
                            {
                                var parameters = new List<SqlParameter>
                                {
                                    new SqlParameter("@Action", "MainAnswer"),
                                    new SqlParameter("@QuestionId", ans.mainQuestionId ?? (object)DBNull.Value),
                                    new SqlParameter("@SubQuestionId", ans.subQuestionId ?? (object)DBNull.Value),
                                    new SqlParameter("@Answer", ans.answer ?? ""),
                                    new SqlParameter("@GauravGuid", gauravId),
                                    new SqlParameter("@GauravDistrictId", gauravDistrictId),
                                    new SqlParameter("@DistrictId", districtId),
                                    new SqlParameter("@CreatedBy", createdBy)
                                };
                                await _iSql.ExecuteProcedure("SP_SaveGauravAnswers", parameters.ToArray());
                            }
                        }
                    }
                    else
                    {
                        foreach (var ans in block.answers)
                        {
                            if (!string.IsNullOrEmpty(ans.subQuestionId))
                            {
                                var parameters = new List<SqlParameter>
                                {
                                    new SqlParameter("@Action", "SubAnswer"),
                                    new SqlParameter("@QuestionId", ans.mainQuestionId),
                                    new SqlParameter("@SubQuestionId", ans.subQuestionId),
                                    new SqlParameter("@Answer", ans.answer ?? ""),
                                    new SqlParameter("@GauravGuid", gauravId),
                                    new SqlParameter("@GauravDistrictId", gauravDistrictId),
                                    new SqlParameter("@DistrictId", districtId),
                                    new SqlParameter("@rowId", ans.rowId),
                                    new SqlParameter("@CreatedBy", createdBy)
                                };
                                await _iSql.ExecuteProcedure("SP_SaveGauravAnswers", parameters.ToArray());
                            }
                            else
                            {
                                var parameters = new List<SqlParameter>
                                {
                                    new SqlParameter("@Action", "MainAnswer"),
                                    new SqlParameter("@QuestionId", ans.mainQuestionId ?? (object)DBNull.Value),
                                    new SqlParameter("@SubQuestionId", ans.subQuestionId ?? (object)DBNull.Value),
                                    new SqlParameter("@Answer", ans.answer ?? ""),
                                    new SqlParameter("@GauravGuid", gauravId),
                                    new SqlParameter("@GauravDistrictId", gauravDistrictId),
                                    new SqlParameter("@DistrictId", districtId),
                                    new SqlParameter("@CreatedBy", createdBy)
                                };
                                await _iSql.ExecuteProcedure("SP_SaveGauravAnswers", parameters.ToArray());
                            }
                        }
                    }
                }

                _result.status = true;
                _result.message = "Answers saved using stored procedure.";
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error saving answers: " + ex.Message;
            }

            return _result;
        }


        public async Task<List<AnswerBlock>> GetAnswersForEditAsync(string gauravId, int districtId, int gauravDistId)
        {
            var param = new List<SqlParameter>
        {
            new SqlParameter("@GauravId", gauravId),
            new SqlParameter("@DistrictId", districtId),
            new SqlParameter("@GauravDistId", gauravDistId)
        };

            var ds = await _iSql.ExecuteProcedure("SP_GetGauravAnswersForEdit", param.ToArray());

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return new List<AnswerBlock>();

            var result = ds.Tables[0].AsEnumerable().GroupBy(row => row["QuestionMasterId"]?.ToString())
                .Select(group => new AnswerBlock
                {
                    questionId = group.Key,
                    answers = group.Select(row => new AnswerItem
                    {
                        mainQuestionId = row["QuestionMasterId"]?.ToString(),
                        subQuestionId = row["SubQuestionId"]?.ToString(),
                        answer = row["AnswerValue"]?.ToString(),
                        rowId = row["rowId"] == DBNull.Value ? null : Convert.ToInt32(row["rowId"]),
                    }).ToList()
                }).ToList();

            return result;
        }

        public async Task<result> GetGauravVivran(int District_Id)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "GetGauravVivran"),
                    new SqlParameter("@DistrictId", District_Id)
                };
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravVivran", param.ToArray());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var _list = ds.Tables[0].AsEnumerable().Select(row => new
                    {
                        gauravDistId = Convert.ToInt32(row["GauravDist_Id"]),
                        guid = row["Guid"]?.ToString() ?? "",
                        districtId = row["District_Id"] != DBNull.Value ? Convert.ToInt32(row["District_Id"]) : (int?)null,
                        gauravId = row["Gaurave_Id"] != DBNull.Value ? Convert.ToInt32(row["Gaurave_Id"]) : (int?)null,
                        gauravName = row["GauravName"] == DBNull.Value ? "" : Convert.ToString(row["GauravName"]),
                        gauravDistname = row["GauravDistName"] == DBNull.Value ? "" : Convert.ToString(row["GauravDistName"]),
                        mangalName = row["MangalName"] == DBNull.Value ? "" : Convert.ToString(row["MangalName"]),
                        vivran = row["Vivran"] == DBNull.Value ? "" : Convert.ToString(row["Vivran"]),
                        finalSubmit = Convert.ToBoolean(row["FinalSubmit"]),
                    }).ToList();

                    _result.status = true;
                    _result.message = "Questions fetched successfully.";
                    _result.data = _list;
                }
                else
                {
                    _result.status = false;
                    _result.message = "No questions found.";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }
            return _result;
        }

        public async Task<result> SaveGauravVivran(List<GauravVivranModel> list, int districtId)
        {
            result _result = new result();
            try
            {
                foreach (var item in list)
                {
                    var param = new List<SqlParameter>
                    {
                        new SqlParameter("@Action", "SaveGauravVivran"),
                        new SqlParameter("@DistrictId", districtId),
                        new SqlParameter("@GauravName", item.gauravName),
                        new SqlParameter("@GauravDistname", item.gauravDistname ?? ""),
                        new SqlParameter("@MangalName", item.mangalName ?? ""),
                        new SqlParameter("@Vivran", item.vivran ?? "")
                    };

                    await _iSql.ExecuteProcedure("SP_ManageGauravVivran", param.ToArray());
                }

                _result.status = true;
                _result.message = "Saved successfully.";
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }

        public async Task<result> SaveGauravMainProfileAnswer(GauravMainProfileModel model, int districtId)
        {
            result _result = new result();
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Save_GauravMainProfileAnswer"),
                    new SqlParameter("@Introduction", model.Introduction ?? string.Empty),
                    new SqlParameter("@DistrictProfile_one", model.DistrictProfileOne ?? string.Empty),
                    new SqlParameter("@DistrictProfile_two", model.DistrictProfileTwo ?? string.Empty),
                    new SqlParameter("@DistrictId",districtId),
                    new SqlParameter("@FinancialYear_Id",2),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? string.Empty)
                };

                await _iSql.ExecuteProcedure("SP_ManageGauravMainProfileAnswer", parameters.ToArray());

                _result.status = true;
                _result.message = "Data saved successfully.";
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }

        //public async Task<result> GetMainGauravProfile(int DistrictId)
        //{
        //    result _result = new result();
        //    mainProfileMO model = new mainProfileMO();
        //    try
        //    {
        //        var param = new List<SqlParameter>
        //        {
        //             new SqlParameter("@Action", "Get_GauravMainProfileAnswer"),
        //            new SqlParameter("@DistrictId", DistrictId)
        //        };

        //        DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravMainProfileAnswer", param.ToArray());

        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            var _list = ds.Tables[0].AsEnumerable().Select(row => new
        //            {
        //                id = Convert.ToInt32(row["GauravMainProfileAnswerId"]),
        //                introduction = row["Introduction"]?.ToString() ?? "",
        //                districtProfile_One = row["DistrictProfile_one"]?.ToString() ?? "",
        //                districtProfile_Two = row["DistrictProfile_two"]?.ToString() ?? "",
        //                districtId = Convert.ToInt32(row["DistrictId"]),
        //                financialYear_Id = Convert.ToInt32(row["FinancialYear_Id"]),
        //                finalSubmit = Convert.ToBoolean(row["FinalSubmit"])
        //            }).ToList();

        //            _result.status = true;
        //            _result.message = "Data fetched successfully.";
        //            _result.data = _list;
        //        }
        //        else
        //        {
        //            _result.status = false;
        //            _result.message = "No data found.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _result.status = false;
        //        _result.message = "Error: " + ex.Message;
        //    }

        //    return _result;
        //}

        public async Task<result> SaveGauravMainProfileAnswer_Final(GauravMainProfileModel model, int districtId)
        {
            result _result = new result();
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Save_GauravMainProfileAnswer_Final"),
                    new SqlParameter("@Introduction", model.Introduction ?? string.Empty),
                    new SqlParameter("@DistrictProfile_one", model.DistrictProfileOne ?? string.Empty),
                    new SqlParameter("@DistrictProfile_two", model.DistrictProfileTwo ?? string.Empty),
                    new SqlParameter("@DistrictId",districtId),
                    new SqlParameter("@FinancialYear_Id",2),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? string.Empty)
                };

                await _iSql.ExecuteProcedure("SP_ManageGauravMainProfileAnswer", parameters.ToArray());

                _result.status = true;
                _result.message = "Data saved successfully.";
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        public async Task<result> SaveGauravVivran_Final(List<GauravVivranModel> list, int districtId)
        {
            result _result = new result();
            try
            {
                foreach (var item in list)
                {
                    var param = new List<SqlParameter>
                    {
                        new SqlParameter("@Action", "SaveGauravVivran_final"),
                        new SqlParameter("@DistrictId", districtId),
                        new SqlParameter("@GauravName", item.gauravName),
                        new SqlParameter("@GauravDistname", item.gauravDistname ?? ""),
                        new SqlParameter("@MangalName", item.mangalName ?? ""),
                        new SqlParameter("@Vivran", item.vivran ?? "")
                    };

                    await _iSql.ExecuteProcedure("SP_ManageGauravVivran", param.ToArray());
                }

                _result.status = true;
                _result.message = "Saved successfully.";
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }

        public async Task<result> Profile_step2_Final(string gauravId, int districtId)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
                    {
                        new SqlParameter("@Action", "FinalSubmit"),
                        new SqlParameter("@DistrictId", districtId),
                        new SqlParameter("@GauravGuid", gauravId),
                    };
                DataSet ds = await _iSql.ExecuteProcedure("SP_SaveGauravAnswers", param.ToArray());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    _result.status = true;
                    _result.message = "Saved successfully.";
                }
                else
                {
                    _result.status = false;
                    _result.message = "Error: in procedure";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }

        public async Task<result> GetStep2_Status(string gauravId, int districtId)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
                    {
                        new SqlParameter("@Action", "GetFinalSubmit"),
                        new SqlParameter("@DistrictId", districtId),
                        new SqlParameter("@GauravGuid", gauravId),
                    };
                DataSet ds = await _iSql.ExecuteProcedure("SP_SaveGauravAnswers", param.ToArray());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToBoolean(ds.Tables[0].Rows[0]["FinalSubmit"]))
                    {
                        _result.status = true;
                        _result.message = "Already final submit";
                    }
                    else
                    {
                        _result.status = false;
                        _result.message = "open for edit";
                    }
                }
                else
                {
                    _result.status = false;
                    _result.message = "open for edit";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }

        #region step1
        public async Task<result> GetStep1Questions(long districtId, string gauravguid)
        {
            result _result = new result();
            List<MainQuestionMO> list = new List<MainQuestionMO>();

            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@GauravGuid",gauravguid)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GetQuestions_step1", param.ToArray());

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    _result.status = false;
                    _result.message = "No questions found.";
                    return _result;
                }

                // 📌 Map questions
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MainQuestionMO
                    {
                        QuestionMasterId = Convert.ToInt32(dr["QuestionMasterId"]),
                        DisplayNumber = dr["DisplayNumber"].ToString(),
                        QuestionText = dr["QuestionText"].ToString(),
                        AnswerValue = dr["AnswerValue"].ToString()
                    });
                }

                // SUCCESS
                _result.status = true;
                _result.message = "Step-1 questions loaded successfully.";
                _result.data = list;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        public async Task<result> SaveStep1Answers(List<MainQuestionMO> answers, int financialYearId, long districtId, string gauravGuid, long userid)
        {
            result _res = new result();
            try
            {
                // Convert List → TVP
                DataTable tvp = new DataTable();
                tvp.Columns.Add("QuestionMasterId", typeof(long));
                tvp.Columns.Add("AnswerValue", typeof(string));

                foreach (var a in answers)
                {
                    tvp.Rows.Add(a.QuestionMasterId, a.AnswerValue ?? "");
                }

                // SQL Parameters
                List<SqlParameter> param = new()
                {
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@GauravGuid", gauravGuid),
                    new SqlParameter("@UserId", userid),
                    new SqlParameter("@FinancialYearId", financialYearId),
                    new SqlParameter("@Answers", tvp)
                    {
                        TypeName = "TVP_Answers",
                        SqlDbType = SqlDbType.Structured
                    }
                };

                // Execute SP
                await _iSql.ExecuteProcedure("SP_SaveAnswers_step1", param.ToArray());

                // Success result
                _res.status = true;
                _res.message = "Step 1 answers saved successfully.";
            }
            catch (Exception ex)
            {
                // Error Result
                _res.status = false;
                _res.message = "Error saving Step 1: " + ex.Message;
            }

            return _res;
        }
        #endregion

        #region step2
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
        public async Task<result> SaveStep2Answers(List<Step2AnswerMO> answers, int financialYearId, long districtId, string gauravGuid, long userId, int rowId)
        {
            result _res = new result();
            try
            {
                // ---------------------------
                // Create Table-Valued Parameter
                // ---------------------------
                DataTable tvp = new DataTable();
                tvp.Columns.Add("QuestionMasterId", typeof(long));
                tvp.Columns.Add("SubQuestionMasterId", typeof(long));
                tvp.Columns.Add("AnswerValue", typeof(string));
                tvp.Columns.Add("RowId", typeof(long));
                tvp.Columns.Add("CreatedBy", typeof(long));

                foreach (var a in answers)
                {
                    tvp.Rows.Add(
                        a.QuestionMasterId,
                        a.SubQuestionMasterId,
                        a.AnswerValue ?? "",
                        a.rowId
                    );
                }

                // ---------------------------
                // SQL Parameters
                // ---------------------------
                List<SqlParameter> param = new()
                {
                   new SqlParameter("@GauravGuid", gauravGuid),
                   new SqlParameter("@DistrictId", districtId),
                   new SqlParameter("@FinancialYearId", financialYearId),
                   new SqlParameter("@rowId", rowId),
                   new SqlParameter("@Answers", tvp)
                   {
                       TypeName = "TVP_Step2Answer",
                       SqlDbType = SqlDbType.Structured
                   }
                };

                // ---------------------------
                // Execute Stored Procedure
                // ---------------------------
                await _iSql.ExecuteProcedure("SP_SaveAnswers_step2", param.ToArray());

                _res.status = true;
                _res.message = "Step 2 answers saved successfully.";
            }
            catch (Exception ex)
            {
                _res.status = false;
                _res.message = "Error saving Step 2: " + ex.Message;
            }

            return _res;
        }
        public async Task<result> GetStep2Answers(int districtId, string gauravGuid, int financialYearId)
        {
            result _result = new result();
            try
            {
                // Prepare parameters
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@GauravGuid", gauravGuid),
                    new SqlParameter("@FinancialYearId", financialYearId)
                };

                // Call Stored Procedure
                DataSet ds = await _iSql.ExecuteProcedure("SP_GetAnswers_step2", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var _list = ds.Tables[0].AsEnumerable().Select(row =>
                    {
                        Step2AnswerMO obj = new Step2AnswerMO
                        {
                            rowId = row["RowId"] != DBNull.Value ? Convert.ToInt32(row["RowId"]) : 0,
                            SubQuestionMasterId = row["SubQuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["SubQuestionMasterId"]) : 0,
                            AnswerValue = row["AnswerValue"]?.ToString() ?? "",
                            QuestionMasterId = row["QuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["QuestionMasterId"]) : 0
                        };

                        if (obj.SubQuestionMasterId == 17)
                        {
                            int enumId = 0;
                            int.TryParse(obj.AnswerValue, out enumId);

                            EnumProfileQuestion enumValue = Enum.IsDefined(typeof(EnumProfileQuestion), enumId)
                                ? (EnumProfileQuestion)enumId
                                : EnumProfileQuestion.OtherActivity;

                            // ⭐ Get display text from enum
                            obj.AnswerValue = GetDisplayName(enumValue);
                        }
                        return obj;
                    }).ToList();

                    _result.status = true;
                    _result.message = "Answers fetched successfully.";
                    _result.data = _list;
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
        public async Task<result> deleteStep2(string rowId, string guid,int districtId,int FyId)
        {
            result _result = new result();
            List<QuestionModel_step2> list = new List<QuestionModel_step2>();

            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@rowId", rowId),
                    new SqlParameter("@GauravGuid", guid),
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@FinancialYearId", FyId)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_DeleteAns_step2", param.ToArray());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    _result.status = true;
                    _result.message = ds.Tables[0].Rows[0][1].ToString();
                }
                else
                {
                    _result.status = false;
                    _result.message = "something went wrong";
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
        #endregion

        #region step3
        public async Task<result> GetStep3Questions(long districtId, string gauravGuid)
        {
            result _result = new result();
            List<MainQuestionStep3MO> model = new List<MainQuestionStep3MO>();

            try
            {
                var param = new List<SqlParameter>
                    {
                        new SqlParameter("@DistrictId", districtId),
                        new SqlParameter("@GauravGuid", gauravGuid)
                    };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GetQuestions_step3", param.ToArray());

                // ds.Tables[0] → Main Questions
                // ds.Tables[1] → Sub Questions + AnswerValue

                if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                {
                    _result.status = false;
                    _result.message = "No Step-3 questions found.";
                    return _result;
                }

                // Map Main Questions
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    model.Add(new MainQuestionStep3MO
                    {
                        QuestionMasterId = Convert.ToInt32(dr["QuestionMasterId"]),
                        Guid = dr["Guid"].ToString(),
                        DistrictId = Convert.ToInt32(dr["DistrictId"]),
                        GauravId = Convert.ToInt32(dr["GauravId"]),
                        QuestionType = dr["QuestionType"].ToString(),
                        DisplayNumber = dr["DisplayNumber"].ToString(),
                        OrderNumber = Convert.ToInt32(dr["OrderNumber"]),
                        QuestionText = dr["QuestionText"].ToString(),
                        IsSubQuestion = Convert.ToBoolean(dr["IsSubQuestion"]),
                        MainQuestionId = Convert.ToInt32(dr["MainQuestionId"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        SubQuestions = new List<MainQuestionStep3MO>()
                    });
                }

                // Map Sub Questions
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        var sub = new MainQuestionStep3MO
                        {
                            QuestionMasterId = Convert.ToInt32(dr["QuestionMasterId"]),
                            Guid = dr["Guid"].ToString(),
                            DistrictId = Convert.ToInt32(dr["DistrictId"]),
                            GauravId = Convert.ToInt32(dr["GauravId"]),
                            QuestionType = dr["QuestionType"].ToString(),
                            DisplayNumber = dr["DisplayNumber"].ToString(),
                            OrderNumber = Convert.ToInt32(dr["OrderNumber"]),
                            QuestionText = dr["QuestionText"].ToString(),
                            IsSubQuestion = Convert.ToBoolean(dr["IsSubQuestion"]),
                            MainQuestionId = Convert.ToInt32(dr["MainQuestionId"]),
                            IsActive = Convert.ToBoolean(dr["IsActive"]),
                            AnswerValue = dr["AnswerValue"]?.ToString() ?? ""   // Saved answer
                        };

                        // Attach to Main Question
                        var parent = model.FirstOrDefault(x => x.QuestionMasterId == sub.MainQuestionId);
                        if (parent != null)
                        {
                            parent.SubQuestions.Add(sub);
                        }
                    }
                }

                // SUCCESS
                _result.status = true;
                _result.message = "Step-3 questions loaded successfully.";
                _result.data = model;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }
            return _result;
        }
        #endregion

        #region main Gaurav Profile
        public async Task<result> GetMainGauravProfile(int DistrictId, int FyId)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action", "Get_GauravMainProfileAnswer"),
                    new SqlParameter("@DistrictId", DistrictId),
                    new SqlParameter("@FinancialYear_Id", FyId)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravMainProfileAnswer", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // ----------- Table 1: Main Profile -----------
                    var mainProfile = ds.Tables[0].AsEnumerable().Select(row => new mainProfileMO
                    {
                        GauravMainProfileAnswerId = row["GauravMainProfileAnswerId"] != DBNull.Value ? Convert.ToInt32(row["GauravMainProfileAnswerId"]) : (int?)null,
                        DistrictId = row["DistrictId"] != DBNull.Value ? Convert.ToInt32(row["DistrictId"]) : (int?)null,
                        FinancialYear_Id = row["FinancialYear_Id"] != DBNull.Value ? Convert.ToInt32(row["FinancialYear_Id"]) : (int?)null,
                        FinalSubmit = row["FinalSubmit"] != DBNull.Value ? Convert.ToInt32(row["FinalSubmit"]) : (int?)null,
                        Introduction = row["Introduction"] != DBNull.Value ? row["Introduction"].ToString() : "",
                        DistrictProfile_one = row["DistrictProfile_one"] != DBNull.Value ? row["DistrictProfile_one"].ToString() : "",
                        DistrictProfile_two = row["DistrictProfile_two"] != DBNull.Value ? row["DistrictProfile_two"].ToString() : "",


                        list = new List<mainProfile_VivranMO>() // Initialize empty, will fill after
                    }).FirstOrDefault();

                    // ----------- Table 2: Gaurav Items / Vivran -----------
                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        mainProfile.list = ds.Tables[1].AsEnumerable().Select(row => new mainProfile_VivranMO
                        {
                            Id = Convert.ToInt32(row["GauravDist_Id"]),
                            name = row["GauravName"].ToString(),
                            GauravVivran = row["Vivran"]?.ToString() ?? "",
                        }).ToList();

                        // Mapping GauravItem manually (Based on row order or business logic)
                        string[] predefinedItems = { "Crop", "Botanical Species", "Product", "Tourist Place", "Sports" };

                        for (int i = 0; i < mainProfile.list.Count && i < predefinedItems.Length; i++)
                        {
                            mainProfile.list[i].GauravItem = predefinedItems[i];
                        }
                    }

                    _result.status = true;
                    _result.message = "Data fetched successfully";
                    _result.data = mainProfile;
                }
                else
                {
                    _result.status = false;
                    _result.message = "No data found";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }

        public async Task<result> SaveMainProfile(mainProfileMO model, string submitType)
        {
            result _res = new result();
            try
            {
                if (submitType == "Verify")
                {
                    model.FinalSubmit = 1;
                }
                else
                {
                    model.FinalSubmit = 0;
                }
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("GauravItem", typeof(string));
                dt.Columns.Add("GauravVivran", typeof(string));

                foreach (var item in model.list)
                {
                    dt.Rows.Add(item.Id, item.name, item.GauravItem, item.GauravVivran);
                }

                var param = new[]
                {
                new SqlParameter("@GauravMainProfileAnswerId", model.GauravMainProfileAnswerId == 0 ? DBNull.Value : model.GauravMainProfileAnswerId ),
                new SqlParameter("@Introduction", model.Introduction ?? ""),
                new SqlParameter("@DistrictProfile_one", model.DistrictProfile_one ?? ""),
                new SqlParameter("@DistrictProfile_two", model.DistrictProfile_two ?? ""),
                new SqlParameter("@DistrictId", model.DistrictId),
                new SqlParameter("@FinancialYear_Id", model.FinancialYear_Id),
                new SqlParameter("@FinalSubmit", model.FinalSubmit),
                new SqlParameter("@CreatedBy", model.CreatedBy),

                    new SqlParameter("@List", dt)
                    {
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "GauravMainProfileTVP"
                    }
                };

                var ds = await _iSql.ExecuteProcedure("SP_SaveGauravMainProfile", param);

                _res.status = true;
                _res.message = "Step 2 answers saved successfully.";
            }
            catch (Exception ex)
            {
                _res.status = false;
                _res.message = "Error saving Step 2: " + ex.Message;
            }

            return _res;
        }

        #endregion

        #region Send,done,back profile verification
        public async Task<result> SendVerification_Profile(int financialYearId, long districtId, string gauravGuid, long userId)
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action","SendVerification"),
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@GauravGuid", gauravGuid),
                    new SqlParameter("@FinancialYearId", financialYearId),
                    new SqlParameter("@CreatedBy", userId),
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GauravProfileVerify", param.ToArray());
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

        public async Task<result> DoneVerification_Profile(int financialYearId, long districtId, string gauravGuid, long userId, string remark)
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action","DoneVerification"),
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@GauravGuid", gauravGuid),
                    new SqlParameter("@FinancialYearId", financialYearId),
                    new SqlParameter("@Remark", remark),
                    new SqlParameter("@CreatedBy", userId),
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GauravProfileVerify", param.ToArray());
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

        public async Task<result> RevertBackVerification_Profile(int financialYearId, long districtId, string gauravGuid, long userId, string remark)
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@Action","RevertBack"),
                    new SqlParameter("@DistrictId", districtId),
                    new SqlParameter("@GauravGuid", gauravGuid),
                    new SqlParameter("@FinancialYearId", financialYearId),
                    new SqlParameter("@Remark", remark),
                    new SqlParameter("@CreatedBy", userId),
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GauravProfileVerify", param.ToArray());
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

        #region Pending Verification list
        public async Task<List<ProfileVerificationPendingVM>> GetProfileVerificationPendingAsync(int financialYearId, long districtId, int deptId)
        {
            var list = new List<ProfileVerificationPendingVM>();

            try
            {
                var param = new List<SqlParameter>
                    {
                        new SqlParameter("@DistrictId", districtId),
                        new SqlParameter("@FinancialYearId", financialYearId),
                        new SqlParameter("@DepartmentId", deptId)
                    };

                DataSet ds = await _iSql.ExecuteProcedure(
                                    "SP_GetProfileVerification_Pending",
                                    param.ToArray());

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new ProfileVerificationPendingVM
                        {
                            DistrictName = Convert.ToString(dr["DISTRICT_ENG"]),
                            GauravName = Convert.ToString(dr["Gaurav_Eng"]),
                            GauravGuid = Convert.ToString(dr["GauravGuid"]),
                            DistrictId = Convert.ToInt64(dr["DistrictId"]),
                            FinancialYearId = Convert.ToInt32(dr["FinancialYear_Id"])
                        });
                    }
                }
            }
            catch (Exception)
            {
                // log exception if logger available
                return new List<ProfileVerificationPendingVM>();
            }
            return list;
        }

        public async Task<ProfileBasicDetail> GetBasicDetail(int financialYearId, string gauravGuid, int districtId)
        {
            try
            {
                var param = new List<SqlParameter>
               {
                   new SqlParameter("@FinancialYearId", financialYearId),
                   new SqlParameter("@GauravGuid", gauravGuid),
                   new SqlParameter("@DistrictId", districtId)
               };

                DataSet ds = await _iSql.ExecuteProcedure(
                                "SP_GET_GauravDistrictItem",
                                param.ToArray());

                var list = new ProfileBasicDetail();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list = new ProfileBasicDetail
                        {
                            GauravEng = dr["GauravEng"]?.ToString(),
                            GauravMangal = dr["GauravMangal"]?.ToString(),
                            GauravItemEng = dr["GauravItemEng"]?.ToString(),
                            GauravItemMangal = dr["GauravItemMangal"]?.ToString(),
                            FinancialYear = dr["FinancialYear"]?.ToString(),
                            District = dr["District"]?.ToString(),
                            DistrictId = Convert.ToInt32(dr["DistrictId"])
                        };
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                // 🔴 Log exception here

                return new ProfileBasicDetail();
            }
        }
        #endregion

        #region District work Plan Trail
        public async Task<result> GetApplicationTrail(int financialYearId, string gauravGuid, int districtId)
        {
            result _result = new result();
            List<ProfileBasicTrails> list = new List<ProfileBasicTrails>();

            try
            {
                var param = new List<SqlParameter>
        {
            new SqlParameter("@Action","ApplicationTrail"),
            new SqlParameter("@DistrictId", districtId),
            new SqlParameter("@FinancialYearId", financialYearId),
            new SqlParameter("@GauravGuid", gauravGuid),
        };

                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_GauravProfileVerify", param.ToArray());

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    _result.status = false;
                    _result.message = "No trail found.";
                    return _result;
                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new ProfileBasicTrails
                    {
                        Remark = dr["Remark"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = dr["CreatedDate"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(dr["CreatedDate"])
                    });
                }

                _result.status = true;
                _result.message = "Application Trail fetched successfully.";
                _result.data = list;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        #endregion

        #region GetApplicationLastRemark
        public async Task<result> GetLastRemark(int financialYearId,string gauravGuid,int districtId)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
        {
            new SqlParameter("@Action", "ApplicationLastRemark"),
            new SqlParameter("@DistrictId", districtId),
            new SqlParameter("@FinancialYearId", financialYearId),
            new SqlParameter("@GauravGuid", gauravGuid)
        };

                DataSet ds = await _iSql.ExecuteProcedure("SP_GauravProfileVerify", param.ToArray());

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    _result.status = false;
                    _result.message = "No remark found.";
                    return _result;
                }

                // ✅ LAST remark = latest CreatedDate
                var lastRow = ds.Tables[0].AsEnumerable()
                    .OrderByDescending(r =>
                        r.Field<DateTime?>("CreatedDate") ?? DateTime.MinValue)
                    .FirstOrDefault();

                _result.status = true;
                _result.data = lastRow?["Remark"]?.ToString();
                _result.message = "Last remark fetched successfully.";
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = ex.Message;
            }

            return _result;
        }

        #endregion
        //#region ProfileVerification
        //public async Task<List<ProfileVerificationMO>> GetProfileVerificationPending(long districtId, int financialYearId)
        //{
        //    List<ProfileVerificationMO> list = new List<ProfileVerificationMO>();

        //    List<SqlParameter> parameters = new List<SqlParameter>
        //        {
        //            new SqlParameter("@DistrictId", districtId),
        //            new SqlParameter("@FinancialYearId", financialYearId)
        //        };

        //    DataSet ds = await _iSql.ExecuteProcedure("SP_GetProfileVerification_Pending", parameters.ToArray());

        //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        list = ds.Tables[0].AsEnumerable().Select(row => new ProfileVerificationMO
        //        {
        //            DistrictEng = row["DISTRICT_ENG"].ToString(),
        //            GauravEng = row["Gaurav_Eng"].ToString(),
        //            GauravId = Convert.ToInt64(row["GauravId"]),
        //            DistrictId = Convert.ToInt64(row["DistrictId"]),
        //            FinancialYearId = Convert.ToInt32(row["FinancialYear_Id"])
        //        }).ToList();
        //    }

        //    return list;
        //}

        //#endregion
    }
}
