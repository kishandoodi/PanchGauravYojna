using DL;
using MO;
using MO.Common;
using MO.Indicator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BL.Progres
{
    public class ProgressReport : IProgressReport
    {
        #region Properties
        private readonly ISQLHelper _iSql;
        #endregion
        #region Constructor
        public ProgressReport(ISQLHelper iSql)
        {
            _iSql = iSql;
        }
        #endregion

        //public async Task<result> GetProgressReportNew(string Id,int District_Id)
        //{
        //    result _result = new result();
        //    List <ProgressMO> _list = new List <ProgressMO>();
        //    try
        //    {
        //        List<SqlParameter> param = new List<SqlParameter>();
        //        param.Add(new SqlParameter("@GauraveId", Id));
        //        param.Add(new SqlParameter("@District_Id", District_Id));

        //        DataSet ds = await _iSql.ExecuteProcedure("SP_ManageProgress", param.ToArray());
        //        {
        //            // htmlstring.AppendLine("<table class='table table-bordered' border='1'></tbody>");
        //            ;
        //            if (ds.Tables.Count > 0)
        //            {
        //                if (ds.Tables[0].Rows.Count > 0)
        //                {
        //                    // All indicators from DB
        //                    var allIndicators = ds.Tables[0].AsEnumerable().Select(s => new ProgressMO
        //                    {
        //                        indicatorId = s["IndicatorId"]?.ToString() ?? "",
        //                        districtId = s["District_Id"]?.ToString() ?? "",
        //                        gauravId = s["Gaurav_Id"]?.ToString() ?? "",
        //                        gauravIdistId = s["GauravDist_Id"]?.ToString() ?? "",
        //                        gauravDistrictName = s["GauravDistrictName"]?.ToString() ?? "",
        //                        indicatorname = s["Indicator"]?.ToString() ?? "",
        //                        unit = s["Unit"]?.ToString() ?? "",
        //                        basevalue = s["Basevalue"]?.ToString() ?? "",
        //                        targetvalue = s["TargetValue"]?.ToString() ?? "",
        //                        indicatortype = s["Indicator_Type"]?.ToString() ?? "",
        //                        group_IndicatorId = s["Group_IndicatorId"]?.ToString() ?? "",
        //                    }).ToList();

        //                    var mainIndicators = allIndicators
        //                             .Where(x => x.indicatortype == "main")
        //                             .ToList();

        //                    var groupMainIndicators = allIndicators
        //                        .Where(x => x.indicatortype == "group_main")
        //                        .ToList();

        //                    foreach (var group in groupMainIndicators)
        //                    {
        //                        group.SubIndicators = allIndicators
        //                            .Where(x => x.indicatortype == "group_part" && x.group_IndicatorId == group.indicatorId)
        //                            .ToList();
        //                    }
        //                    //_list = ds.Tables[0].AsEnumerable().Select(s => new ProgressMO
        //                    //{
        //                    //    //Id = s["Id"] == DBNull.Value ? 0 : Convert.ToInt32(s["Id"]),
        //                    //    indicatorId = s["IndicatorId"] == DBNull.Value ? "" : Convert.ToString(s["IndicatorId"]),                                
        //                    //    districtId = s["District_Id"] == DBNull.Value ? "" : Convert.ToString(s["District_Id"]),
        //                    //    gauravId = s["Gaurav_Id"] == DBNull.Value ? "" : Convert.ToString(s["Gaurav_Id"]),
        //                    //    gauravIdistId = s["GauravDist_Id"] == DBNull.Value ? "" : Convert.ToString(s["GauravDist_Id"]),
        //                    //    gauravDistrictName = s["GauravDistrictName"] == DBNull.Value ? "" : Convert.ToString(s["GauravDistrictName"]),
        //                    //    indicatorname = s["Indicator"] == DBNull.Value ? "" : Convert.ToString(s["Indicator"]),
        //                    //    unit = s["Unit"] == DBNull.Value ? "" : Convert.ToString(s["Unit"]),   
        //                    //    basevalue = s["Basevalue"] == DBNull.Value ? "" : Convert.ToString(s["Basevalue"]),
        //                    //    targetvalue = s["TargetValue"] == DBNull.Value ? "" : Convert.ToString(s["TargetValue"]),
        //                    //    indicatortype = s["Indicator_Type"] == DBNull.Value ? "" : Convert.ToString(s["Indicator_Type"]),
        //                    //    group_IndicatorId = s["Group_IndicatorId"] == DBNull.Value ? "" : Convert.ToString(s["Group_IndicatorId"]),
        //                    //}).Where(x=> x.indicatortype == 'main' || x.indicatortype == '').ToList();
        //                }
        //                _result.status = true;
        //                _result.message = "data found !";
        //                _result.data = _list;
        //            }
        //        }

        //        return  _result;                
        //    }
        //    catch (Exception ex)
        //    {
        //        return new result();
        //        };
        //    }
        public async Task<result> GetProgressReportNew(string Id, int District_Id)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
            {
                new SqlParameter("@GauraveId", Id),
                new SqlParameter("@District_Id", District_Id)
            };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageProgress", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    // Step 1: Map all indicators from DB
                    var allIndicators = ds.Tables[0].AsEnumerable().Select(s => new ProgressMO
                    {
                        indicatorId = s["IndicatorId"]?.ToString() ?? "",
                        districtId = s["District_Id"]?.ToString() ?? "",
                        gauravId = s["Gaurav_Id"]?.ToString() ?? "",
                        gauravIdistId = s["GauravDist_Id"]?.ToString() ?? "",
                        gauravDistrictName = s["GauravDistrictName"]?.ToString() ?? "",
                        indicatorname = s["Indicator"]?.ToString() ?? "",
                        unit = s["Unit"]?.ToString() ?? "",
                        basevalue = s["Basevalue"]?.ToString() ?? "",
                        targetvalue = s["TargetValue"]?.ToString() ?? "",
                        indicatortype = s["Indicator_Type"]?.ToString() ?? "",
                        group_IndicatorId = s["Group_IndicatorId"]?.ToString() ?? ""
                    }).ToList();

                    // Step 2: Create main indicator list
                    var resultList = new List<ProgressMO>();

                    // Add direct main indicators (no grouping)
                    var mainIndicators = allIndicators
                        .Where(x => x.indicatortype == "main")
                        .ToList();
                    resultList.AddRange(mainIndicators);

                    // Add group_main with group_part
                    var groupMains = allIndicators
                        .Where(x => x.indicatortype == "group_main")
                        .ToList();

                    foreach (var group in groupMains)
                    {
                        group.SubIndicators = allIndicators
                            .Where(x => x.indicatortype == "group_part" && x.group_IndicatorId == group.indicatorId)
                            .ToList();

                        resultList.Add(group); // insert as group header
                    }

                    _result.status = true;
                    _result.message = "Data found!";
                    _result.data = resultList;
                }
                else
                {
                    _result.status = false;
                    _result.message = "No data found.";
                }
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Exception: " + ex.Message;
            }

            return _result;
        }

        public async Task<result> SaveIndicatorTargetMaster(List<IndicatorTargetMaster> model)
        {
            result _result = new result();

            try
            {
                DataTable table = new DataTable();
                //table.Columns.Add("IndicatorTargetMasterId", typeof(int));
                table.Columns.Add("FinancialYear_Id", typeof(int));
                table.Columns.Add("District_Id", typeof(int));
                table.Columns.Add("Gaurav_Id", typeof(int));
                table.Columns.Add("GauravDist_Id", typeof(long));
                table.Columns.Add("Indicator_Id", typeof(long));
                table.Columns.Add("Basevalue", typeof(string));
                table.Columns.Add("TargetValue", typeof(string));
                table.Columns.Add("Remark", typeof(string));
                table.Columns.Add("CreatedBy", typeof(string));

                foreach (var item in model)
                {
                    table.Rows.Add(
                        item.FinancialYear_Id,
                        Convert.ToInt32(item.District_Id),
                        Convert.ToInt32(item.Gaurav_Id),
                        Convert.ToInt64(item.GauravDist_Id),
                        Convert.ToInt64(item.Indicator_Id),
                        item.Basevalue ?? "",
                        item.TargetValue ?? "",
                        item.Remark ?? "",
                        item.CreatedBy ?? ""
                    );
                }

                var param = new SqlParameter("@IndicatorTargetList", SqlDbType.Structured)
                {
                    TypeName = "dbo.IndicatorTargetMasterType",
                    Value = table
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageIndicatorTargetMasterBulk", new[] { param });

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _result.status = Convert.ToBoolean(ds.Tables[0].Rows[0]["status"]);
                    _result.message = Convert.ToString(ds.Tables[0].Rows[0]["message"]);
                }

                return _result;
            }
            catch (Exception ex)
            {
                return new result { status = false, message = "An error occurred during bulk save." };
            }
        }

        #region Question Gaurav Master
        public async Task<result> GetQuestionReport(string gauravId, int District_Id)
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@GauravId", gauravId),
                    new SqlParameter("@District_Id", District_Id)
                };
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageQuestionsGaurav", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var allQuestions = ds.Tables[0].AsEnumerable().Select(row => new QuestionMasterMO
                    {
                        DisplayNumber = row["DisplayNumber"]?.ToString() ?? "",
                        QuestionText = row["QuestionText"]?.ToString() ?? "",
                        QuestionType = row["QuestionType"] != DBNull.Value ? Convert.ToInt32(row["QuestionType"]) : (int?)null,
                        IsSubQuestion = row["IsSubQuestion"] != DBNull.Value && Convert.ToBoolean(row["IsSubQuestion"]),
                        OrderNumber = row["OrderNumber"] == DBNull.Value ? 0 : Convert.ToInt32(row["OrderNumber"]),
                        SubQuestions = new List<SubQuestionMasterMO>(),
                        // Add this only if you want to track parent linking
                        MainQuestionId = row["MainQuestionId"] != DBNull.Value ? Convert.ToInt32(row["MainQuestionId"]) : (int?)null,
                        QuestionMasterId = Convert.ToInt32(row["QuestionMasterId"])
                    }).ToList();

                    var subQuestiontb = new List<subQuestiontbl>();
                    var subQuestionsTbl = ds.Tables[1].AsEnumerable().Select(row => new subQuestiontbl
                    {
                        QuestionText = row["QuestionText"]?.ToString() ?? "",
                        QuestionType = row["QuestionType"] != DBNull.Value ? Convert.ToInt32(row["QuestionType"]) : (int?)null,
                        OrderNumber = row["OrderNumber"] != DBNull.Value ? Convert.ToInt32(row["OrderNumber"]) : (int?)null,
                        SubQuestionMasterId = Convert.ToInt32(row["SubQuestionMasterId"]),
                        Guid = row["Guid"] != DBNull.Value ? Convert.ToString(row["Guid"]) : null,
                    }).OrderBy(x=> x.OrderNumber).ToList();

                    // Grouping logic
                    var resultList = new List<QuestionMasterMO>();

                    var mainQuestions = allQuestions
                        .Where(q => (!q.IsSubQuestion && q.MainQuestionId == 0) || (q.IsSubQuestion && q.MainQuestionId == 0))
                        .ToList();

                    foreach (var main in mainQuestions)
                    {
                        main.SubQuestions = allQuestions
                        .Where(sub => !sub.IsSubQuestion && sub.MainQuestionId == main.QuestionMasterId)
                        .Select(sub => new SubQuestionMasterMO
                        {
                            QuestionMasterId = sub.QuestionMasterId,
                            Guid = sub.Guid,
                            DistrictId = sub.DistrictId,
                            GauravId = sub.GauravId,
                            QuestionType = sub.QuestionType,
                            OrderNumber = sub.OrderNumber,
                            QuestionText = sub.QuestionText,
                            DisplayNumber = sub.DisplayNumber,
                            IsSubQuestion = sub.IsSubQuestion,
                            MainQuestionId = sub.MainQuestionId
                        })
                        .ToList();

                        foreach (var sc in main.SubQuestions)
                        {
                            sc.SubColumn = subQuestionsTbl.Where(sf => sf.QuestionType == sc.QuestionType).ToList();
                        }
                        //main.subtblcolumn = subQuestionsTbl;

                        main.SubColumn = subQuestionsTbl.Where(sfd => sfd.QuestionType == main.QuestionType).ToList();
                    }

                    _result.status = true;
                    _result.message = "Questions fetched successfully.";
                    _result.data = mainQuestions;
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

        public async Task<result> saveGauravProfile(int gauravid, int distid, int gauravdistid, int financialyearid, string questionId, string answer, string type, string column = null, string createdBy = "System")
        {
            result _result = new result();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@QuestionId", questionId),
                    new SqlParameter("@AnswerValue", answer),
                    new SqlParameter("@Type", type),
                    new SqlParameter("@ColumnName", column),
                    new SqlParameter("@CreatedBy", createdBy),
                    new SqlParameter("@GauravId", gauravid),
                    new SqlParameter("@DistrictId", distid),
                    new SqlParameter("@GauravDistId", gauravdistid),
                    new SqlParameter("@FinancialYear_Id", financialyearid),
                };
                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageGauravProfileSave", param.ToArray());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _result.status = Convert.ToBoolean(ds.Tables[0].Rows[0]["status"]);
                    _result.message = Convert.ToString(ds.Tables[0].Rows[0]["message"]);
                }

                return _result;
            }
            catch (Exception ex)
            {
                return new result { status = false, message = "An error occurred during bulk save." };
            }
        }
        #endregion

        #region NEW 
        public async Task<result> GetQuestionReport_New(string gauravId, int District_Id)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@GauravId", gauravId),
                    new SqlParameter("@District_Id", District_Id)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageQuestionsGauravProfile", param.ToArray());

                if (ds.Tables.Count < 2)
                {
                    _result.status = false;
                    _result.message = "Invalid dataset returned from SP.";
                    return _result;
                }

                // =======================
                // 1️⃣ MAP MAIN QUESTIONS
                // =======================

                var allQuestions = ds.Tables[0].AsEnumerable().Select(row => new QuestionMasterMO
                {
                    QuestionMasterId = row["QuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["QuestionMasterId"]) : 0,
                    DisplayNumber = row["DisplayNumber"]?.ToString(),
                    QuestionText = row["QuestionText"]?.ToString(),
                    QuestionType = row["QuestionType"] != DBNull.Value ? Convert.ToInt32(row["QuestionType"]) : (int?)null,
                    IsSubQuestion = row["IsSubQuestion"] != DBNull.Value && Convert.ToBoolean(row["IsSubQuestion"]),
                    MainQuestionId = row["MainQuestionId"] != DBNull.Value ? Convert.ToInt32(row["MainQuestionId"]) : (int?)null,
                    OrderNumber = row["OrderNumber"] != DBNull.Value ? Convert.ToInt32(row["OrderNumber"]) : 0,

                    // NEW → answer mapping
                    AnswerValue = row["AnswerValue"]?.ToString(),
                    AnswerSubId = row["AnswerSubId"] != DBNull.Value ? Convert.ToInt32(row["AnswerSubId"]) : (int?)null,
                    rowId = row["rowId"] != DBNull.Value ? Convert.ToInt32(row["rowId"]) : (int?)null,

                    SubQuestions = new List<SubQuestionMasterMO>(),
                    SubColumn = new List<subQuestiontbl>()
                }).OrderBy(q => q.OrderNumber).ToList();



                // =======================
                // 2️⃣ MAP SUB QUESTIONS
                // =======================

                var subQuestionsTbl = ds.Tables[1].AsEnumerable().Select(row => new subQuestiontbl
                {
                    SubQuestionMasterId = Convert.ToInt32(row["SubQuestionMasterId"]),
                    QuestionText = row["QuestionText"]?.ToString(),
                    QuestionType = row["QuestionType"] != DBNull.Value ? Convert.ToInt32(row["QuestionType"]) : (int?)null,
                    OrderNumber = row["OrderNumber"] != DBNull.Value ? Convert.ToInt32(row["OrderNumber"]) : 0,
                    Guid = row["Guid"]?.ToString(),

                    // NEW → answers for subquestions
                    AnswerValue = row["AnswerValue"]?.ToString(),
                    rowId = row["rowId"] != DBNull.Value ? Convert.ToInt32(row["rowId"]) : (int?)null,

                }).ToList();

                // =======================
                // 3️⃣ GROUPING HIERARCHY
                // =======================
                var mainQuestions = allQuestions
                    .Where(q => q.MainQuestionId == 0)
                    .ToList();

                //var mainQuestions = allQuestions
                //        .Where(q => (!q.IsSubQuestion && q.MainQuestionId == 0) || (q.IsSubQuestion && q.MainQuestionId == 0))
                //        .ToList();              
                foreach (var main in mainQuestions)
                {
                    main.SubQuestions = allQuestions
                    .Where(sub => !sub.IsSubQuestion && sub.MainQuestionId == main.QuestionMasterId)
                    .Select(sub => new SubQuestionMasterMO
                    {
                        QuestionMasterId = sub.QuestionMasterId,
                        Guid = sub.Guid,
                        DistrictId = sub.DistrictId,
                        GauravId = sub.GauravId,
                        QuestionType = sub.QuestionType,
                        OrderNumber = sub.OrderNumber,
                        QuestionText = sub.QuestionText,
                        DisplayNumber = sub.DisplayNumber,
                        IsSubQuestion = sub.IsSubQuestion,
                        MainQuestionId = sub.MainQuestionId,
                        AnswerSubId = sub.AnswerSubId,
                        AnswerValue = sub.AnswerValue,
                        rowId = sub.rowId,                      
                    })
                    .ToList();

                    foreach (var sc in main.SubQuestions)
                    {
                        sc.SubColumn = subQuestionsTbl.Where(sf => sf.QuestionType == sc.QuestionType).ToList();
                    }
                    //main.subtblcolumn = subQuestionsTbl;

                    main.SubColumn = subQuestionsTbl.Where(sfd => sfd.QuestionType == main.QuestionType).ToList();
                }  

                // =======================
                // 4️⃣ FINAL RESPONSE
                // =======================

                _result.status = true;
                _result.message = "Questions fetched successfully.";
                _result.data = mainQuestions;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        public async Task<result> GetQuestionReport_New1(string gauravId, int District_Id)
        {
            result _result = new result();

            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@GauravId", gauravId),
                    new SqlParameter("@District_Id", District_Id)
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_ManageQuestionsGauravProfile1", param.ToArray());

                if (ds.Tables.Count < 2)
                {
                    _result.status = false;
                    _result.message = "Invalid dataset returned from SP.";
                    return _result;
                }

                // =======================
                // 1️⃣ MAP MAIN QUESTIONS
                // =======================

                var allQuestions = ds.Tables[0].AsEnumerable().Select(row => new QuestionMasterMO
                {
                    QuestionMasterId = row["QuestionMasterId"] != DBNull.Value ? Convert.ToInt32(row["QuestionMasterId"]) : 0,
                    DisplayNumber = row["DisplayNumber"]?.ToString(),
                    QuestionText = row["QuestionText"]?.ToString(),
                    QuestionType = row["QuestionType"] != DBNull.Value ? Convert.ToInt32(row["QuestionType"]) : (int?)null,
                    IsSubQuestion = row["IsSubQuestion"] != DBNull.Value && Convert.ToBoolean(row["IsSubQuestion"]),
                    MainQuestionId = row["MainQuestionId"] != DBNull.Value ? Convert.ToInt32(row["MainQuestionId"]) : (int?)null,
                    OrderNumber = row["OrderNumber"] != DBNull.Value ? Convert.ToInt32(row["OrderNumber"]) : 0,

                    // NEW → answer mapping
                    //AnswerValue = row["AnswerValue"]?.ToString(),
                    //AnswerSubId = row["AnswerSubId"] != DBNull.Value ? Convert.ToInt32(row["AnswerSubId"]) : (int?)null,
                    //rowId = row["rowId"] != DBNull.Value ? Convert.ToInt32(row["rowId"]) : (int?)null,

                    SubQuestions = new List<SubQuestionMasterMO>(),
                    SubColumn = new List<subQuestiontbl>()
                }).OrderBy(q => q.OrderNumber).ToList();



                // =======================
                // 2️⃣ MAP SUB QUESTIONS
                // =======================

                var subQuestionsTbl = ds.Tables[1].AsEnumerable().Select(row => new subQuestiontbl
                {
                    SubQuestionMasterId = Convert.ToInt32(row["SubQuestionMasterId"]),
                    QuestionText = row["QuestionText"]?.ToString(),
                    QuestionType = row["QuestionType"] != DBNull.Value ? Convert.ToInt32(row["QuestionType"]) : (int?)null,
                    OrderNumber = row["OrderNumber"] != DBNull.Value ? Convert.ToInt32(row["OrderNumber"]) : 0,
                    Guid = row["Guid"]?.ToString(),

                    // NEW → answers for subquestions
                    //AnswerValue = row["AnswerValue"]?.ToString(),
                    //rowId = row["rowId"] != DBNull.Value ? Convert.ToInt32(row["rowId"]) : (int?)null,

                }).ToList();

                // 3️⃣ MAP ANSWERS
                var allAnswers = ds.Tables[2].AsEnumerable().Select(row => new GauravAnswerMO
                {
                    QuestionMasterId = Convert.ToInt32(row["QuestionMasterId"]),
                    SubQuestionId = row["SubQuestionId"] != DBNull.Value ? Convert.ToInt32(row["SubQuestionId"]) : (int?)null,
                    AnswerValue = row["AnswerValue"]?.ToString(),
                    rowId = row["rowId"] != DBNull.Value ? Convert.ToInt32(row["rowId"]) : 0
                }).ToList();

                // =======================
                // 3️⃣ GROUPING HIERARCHY
                // =======================
                var mainQuestions = allQuestions
                    .Where(q => q.MainQuestionId == 0)
                    .ToList();

                foreach (var main in mainQuestions)
                {
                    // 1️⃣ Add SubQuestions (same as earlier)
                    main.SubQuestions = allQuestions
                        .Where(sub => !sub.IsSubQuestion && sub.MainQuestionId == main.QuestionMasterId)
                        .Select(sub => new SubQuestionMasterMO
                        {
                            QuestionMasterId = sub.QuestionMasterId,
                            Guid = sub.Guid,
                            DistrictId = sub.DistrictId,
                            GauravId = sub.GauravId,
                            QuestionType = sub.QuestionType,
                            OrderNumber = sub.OrderNumber,
                            QuestionText = sub.QuestionText,
                            DisplayNumber = sub.DisplayNumber,
                            IsSubQuestion = sub.IsSubQuestion,
                            MainQuestionId = sub.MainQuestionId,
                        }).ToList();

                    var mainAns = allAnswers
                    .FirstOrDefault(a => a.QuestionMasterId == main.QuestionMasterId && a.SubQuestionId == null);

                    if (mainAns != null)
                        main.AnswerValue = mainAns.AnswerValue;

                    foreach (var sc in main.SubQuestions)
                    {
                        // SUB QUESTION ANSWER
                        var subAns = allAnswers
                            .FirstOrDefault(a => a.QuestionMasterId == sc.QuestionMasterId && a.SubQuestionId == null);

                        if (subAns != null)
                            sc.AnswerValue = subAns.AnswerValue;

                        // SUB COLUMNS + ANSWERS
                        sc.SubColumn = subQuestionsTbl
                            .Where(sf => sf.QuestionType == sc.QuestionType)
                            .ToList();

                        foreach (var col in sc.SubColumn)
                        {
                            var colAns = allAnswers
                                .FirstOrDefault(a => a.SubQuestionId == col.SubQuestionMasterId);

                            if (colAns != null)
                                col.AnswerValue = colAns.AnswerValue;
                        }
                    }

                    // 2️⃣ Attach Sub Columns
                    foreach (var sc in main.SubQuestions)
                    {
                        sc.SubColumn = subQuestionsTbl.Where(sf => sf.QuestionType == sc.QuestionType).ToList();
                    }

                    // 3️⃣ Attach main-level Sub Columns
                    main.SubColumn = subQuestionsTbl.Where(sfd => sfd.QuestionType == main.QuestionType).ToList();

                    // 4️⃣ Attach MULTIPLE ANSWER ROWS
                    main.SubColumnRows = allAnswers
                        .Where(a => a.QuestionMasterId == main.QuestionMasterId)
                        .GroupBy(a => a.rowId)
                        .Select(g => g.ToList())
                        .ToList();
                }


                // =======================
                // 4️⃣ FINAL RESPONSE
                // =======================

                _result.status = true;
                _result.message = "Questions fetched successfully.";
                _result.data = mainQuestions;
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = "Error: " + ex.Message;
            }

            return _result;
        }
        #endregion
    }
}
