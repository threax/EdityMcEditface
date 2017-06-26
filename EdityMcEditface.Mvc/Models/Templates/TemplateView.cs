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
    public class TemplateView
    {
        private String path;

        public String Path
        {
            get
            {
                return this.path;
            }
            set
            {
                //Since this gets put into a url, get rid of the starting path chars.
                if(value != null)
                {
                    this.path = value.TrimStartingPathChars();
                }
                else
                {
                    this.path = value;
                }
            }
        }
    }
}
