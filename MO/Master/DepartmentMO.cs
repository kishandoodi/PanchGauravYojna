using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO.Master
{
    public class DepartmentMO
    {
        public int? DepartmentId { get; set; }

        public string? Guid { get; set; }

        //[Required(ErrorMessage = "Code is required")]
        //[MaxLength(50)]
        //public string Code { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }

        // Button name (Save / Update)
        public string? BtnSave { get; set; } = "Save";

        public List<DepartmentMO>? Department_list { get; set; } 
    }
}
