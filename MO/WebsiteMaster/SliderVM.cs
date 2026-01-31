using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO.WebsiteMaster
{
    public class SliderCreateVM
    {
        [ValidateNever]
        public int? Id { get; set; }
        [ValidateNever]
        public string ExistingImage { get; set; }
        [ValidateNever]
        public IFormFile ImageFile { get; set; }
        public string Title { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        [ValidateNever]
        public List<SliderListVM> SliderList { get; set; }
    }
    public class mst_Slider
    {
        public int? Id { get; set; }
        public string ImageBase64 { get; set; }
        public string Title { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
    public class SliderListVM
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageBase64 { get; set; }
    }
    public class HomeSliderVM
    {
        public string ImageBase64 { get; set; }
        public string Title { get; set; }
        public int? DisplayOrder { get; set; }
    }
    public class ToggleSliderVM
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

}
