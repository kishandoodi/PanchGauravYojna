using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO.BudgetMaster
{
    public class VettingList
    {
        public int Id { get; set; }
        public string Gatividhi { get; set; }
        public string GatividhiName { get; set; }
        public string Budget { get; set; }
        public int QuestionMasterId { get; set; }
        public int SubQuestionMasterId { get; set; }
        public string AnswerValue { get; set; }
        public long GauravId { get; set; }
        public long CreatedBy { get; set; }
        public long rowId { get; set; }
        public int DistrictId { get; set; }
        public string Fieldtype { get; set; }
    }
    public class VettingRowVM
    {
        public int RowId { get; set; }
        public int GauravId { get; set; }
        public int DistrictId { get; set; }
        public int QuestionMasterId { get; set; }
        public int SubQuestionMasterId { get; set; }
        public string AnswerValue { get; set; }
        public string Fieldtype { get; set; }
        public Dictionary<int, string> Columns { get; set; } = new();
        public List<VettingDetailVM> Details { get; set; } = new();
    }
    public class VettingDetailVM
    {
        public int QuestionMasterId { get; set; }
        public int SubQuestionMasterId { get; set; }
        public string AnswerValue { get; set; }
        public string Fieldtype { get; set; }
    }
    public class VettingSaveVM
    {
        public int? RowId { get; set; }
        public string Activity { get; set; }
        public string ActivityName { get; set; }
        public string Budget { get; set; }
        public int? GauravId { get; set; }
        public int? DistrictId { get; set; }
        public int? QuestionMasterId { get; set; }
        public int? SubQuestionMasterId { get; set; }
        public int? FinancialYear_Id { get; set; }
    }
    public class VettedQuestions
    {
        public int CurrentStep { get; set; }
        public string GauravGuid { get; set; }
        public string statusClass { get; set; }
        public List<QuestionModel_VettedQuestions> mainquestion { get; set; }
    }
    public class QuestionModel_VettedQuestions
    {
        public int CurrentStep { get; set; }
        public long QuestionMasterId { get; set; }
        public string Guid { get; set; }
        public string DisplayNumber { get; set; }
        public int OrderNumber { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }


        public List<SubQuestionModel_VettedQuestions> SubQuestions { get; set; } = new();
    }

    public class SubQuestionModel_VettedQuestions
    {
        public long SubQuestionMasterId { get; set; }
        public string Guid { get; set; }
        public string DisplayNumber { get; set; }
        public int OrderNumber { get; set; }
        public string QuestionText { get; set; }
        public string Fieldtype { get; set; }
        public int QuestionType { get; set; }
    }
    public class VettedQuestionsSaveModel
    {
        public string ActivityId { get; set; }
        public string activityText { get; set; }
        public string ActivityName { get; set; }
        public string TotalProposed { get; set; }
        public string NodalAmount { get; set; }
        public string MPLADAmount { get; set; }
        public string CSRAmount { get; set; }
        public string OtherAmount { get; set; }
        public string PanchGauravAmount { get; set; }
        public string WorkPlan { get; set; }
        public string CompletionDate { get; set; }
        //public int QuestionMasterId { get; set; }
        //public int SubQuestionMasterId { get; set; }
        //public string AnswerValue { get; set; }
        //public long GauravId { get; set; }
        //public long CreatedBy { get; set; }
        //public long rowId { get; set; }
    }
    public class SaveVettedRequest
    {
        public List<VettedQuestionsSaveModel> Answers { get; set; }
        public VettedAmountModel ModelData { get; set; }
    }
    public class VettedAmountModel
    {
        public string NodalAmount { get; set; }
        public string MPLADAmount { get; set; }
        public string CSRAmount { get; set; }
        public string OtherAmount { get; set; }
        public string PanchGauravAmount { get; set; }
        public string TotalProposed { get; set; }
    }


}
