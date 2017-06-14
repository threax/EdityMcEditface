using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Page
{
    [HalModel]
    [HalSelfActionLink(PageController.Rels.GetSettings, typeof(PageController))]
    [HalActionLink(PageController.Rels.UpdateSettings, typeof(PageController))]
    [HalActionLink(PageController.Rels.Save, typeof(PageController))]
    [HalActionLink(PageController.Rels.Delete, typeof(PageController))]
    public class PageSettings
    {
        [Required]
        public String Title { get; set; }

        public bool Visible { get; set; }

        [JsonIgnore]
        public String FilePath { get; set; }
    }
}
