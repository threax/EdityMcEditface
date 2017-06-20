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
    [HalActionLink(PublishController.Rels.BeginPublish, typeof(PublishController))]
    public class CompileResult
    {
        public double ElapsedSeconds { get; set; }
    }
}
