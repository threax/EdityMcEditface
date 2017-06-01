using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalActionLink(DraftController.Rels.SubmitLatestDraft, typeof(DraftController))]
    public class Draft
    {
        public String File { get; set; }
    }
}
