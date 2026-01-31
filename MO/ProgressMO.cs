using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO
{
    //public class ProgressMO
    //{
    //    public int Id { get; set; }
    //    public string indicatorId { get; set; }
    //    public string districtId { get; set; }  
    //    public string gauravId { get; set; }
    //    public string gauravIdistId { get; set; }
    //    public string gauravDistrictName { get; set; }
    //    public string indicatorname { get; set; }
    //    public string unit { get; set; }
    //    public string status { get; set; }
    //    public string target { get; set; }
    //    public string basevalue { get; set; }   
    //    public string targetvalue { get; set; }
    //    public string indicatortype { get; set; }
    //    public string group_IndicatorId { get; set; }   
    //}

    public class ProgressMO
    {
        public int Id { get; set; }
        public string indicatorId { get; set; }
        public string districtId { get; set; }
        public string gauravId { get; set; }
        public string gauravIdistId { get; set; }
        public string gauravDistrictName { get; set; }
        public string indicatorname { get; set; }
        public string unit { get; set; }
        public string basevalue { get; set; }
        public string targetvalue { get; set; }
        public string indicatortype { get; set; }
        public string group_IndicatorId { get; set; }

        // Sub-indicators (only for group_main)
        public List<ProgressMO> SubIndicators { get; set; } = new();
    }

    public class IndicatorTargetMaster
    {
        public string? IndicatorTargetMasterId { get; set; } 
        public string? FinancialYear_Id { get; set; }
        public string? District_Id { get; set; }
        public string? Gaurav_Id { get; set; }
        public string? GauravDist_Id { get; set; }
        public string? Indicator_Id { get; set; }
        public string? Unit { get; set; }
        public string? Basevalue { get; set; }
        public string? TargetValue { get; set; }
        public string? Remark { get; set; }
        public string? CreatedBy { get; set; }
    }
}
