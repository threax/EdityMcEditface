using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Prepublish
{
    [HalModel]
    [HalActionLink(PrepublishController.Rels.Prepublish, typeof(PrepublishController))]
    public class PrepublishFileInfo
    {
        public String File { get; set; }
    }
}
