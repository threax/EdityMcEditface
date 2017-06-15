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
    [HalSelfActionLink(TemplateController.Rels.List, typeof(TemplateController))]
    public class TemplateCollection : CollectionView<TemplateView>
    {
        public TemplateCollection(IEnumerable<TemplateView> items) : base(items)
        {
        }
    }
}
