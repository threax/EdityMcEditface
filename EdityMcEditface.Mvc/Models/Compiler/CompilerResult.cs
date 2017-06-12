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
    [HalActionLink(PublishController.Rels.PublishStatus, typeof(PublishController))]
    public class CompilerResult
    {
        public double ElapsedSeconds { get; set; }
    }
}
