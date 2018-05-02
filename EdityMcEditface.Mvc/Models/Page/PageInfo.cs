using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Page
{
    [HalModel]
    [HalActionLink(PageController.Rels.Save, typeof(PageController))]
    [HalActionLink(PageController.Rels.Delete, typeof(PageController))]
    [HalActionLink(PageSettingsController.Rels.GetSettings, typeof(PageSettingsController))]
    [HalActionLink(PageSettingsController.Rels.UpdateSettings, typeof(PageSettingsController))]
    public class PageInfo
    {
        public PageInfo()
        {

        }

        public PageInfo(String filePath)
        {
            if (filePath != null)
            {
                this.FilePath = filePath.TrimStartingPathChars();
            }
        }

        public String FilePath { get; set; }
    }
}
