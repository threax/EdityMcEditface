using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Compiler
{
    [HalModel]
    [HalSelfActionLink(PublishController.Rels.Progress, typeof(PublishController))]
    public class CompileProgress : BuildProgress
    {
        public bool Completed { get; set; }
    }
}
