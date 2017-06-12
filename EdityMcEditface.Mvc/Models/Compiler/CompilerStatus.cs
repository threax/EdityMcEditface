using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Compiler
{
    [HalModel]
    [HalSelfActionLink(PublishController.Rels.PublishStatus, typeof(PublishController))]
    public class CompilerStatus
    {
        public int BehindBy { get; internal set; }
        public object BehindHistory { get; internal set; }
    }
}
