using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class PageSettings
    {
        [Required]
        public String Title { get; set; }

        public bool Visible { get; set; }
    }
}
