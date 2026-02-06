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
    }
    public class VettingRowVM
    {
        public int RowId { get; set; }
        public Dictionary<int, string> Columns { get; set; } = new();
    }
}
