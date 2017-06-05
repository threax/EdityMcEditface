using EdityMcEditface.HtmlRenderer.Filesystem;
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
    [HalActionLink(DraftController.Rels.SubmitAllDrafts, typeof(DraftController))]
    public class Draft
    {
        public DateTime? LastUpdate { get; set; }

        public DraftStatus Status { get; set; }

        public String File { get; set; }

        public String Title { get; set; }
    }
}
