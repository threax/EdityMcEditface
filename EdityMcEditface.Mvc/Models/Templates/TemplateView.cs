using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Templates
{
    [HalModel]
    [HalActionLink(TemplateController.Rels.GetContent, typeof(TemplateController))]
    public class TemplateView : Template
    {
    }
}
