using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO.WebsiteMaster
{
    public class AnnouncementVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IFormFile File { get; set; }   // image or pdf
        public string ExistingFilePath { get; set; }

        public bool IsActive { get; set; }
        public bool IsNew { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
