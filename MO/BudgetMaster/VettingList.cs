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
        public int RowId { get; set; }
        public int ColumnIndex { get; set; }
        public string Value { get; set; }
    }

}
