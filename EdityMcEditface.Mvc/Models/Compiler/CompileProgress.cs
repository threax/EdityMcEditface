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
        /// <summary>
        /// Is the compile process complete for this build. This does not indicate if the process was sucessful
        /// just that it is no longer running.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// True if the build was a success, false if it failed. Only valid if Completed is true.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// If success is false, this will be set to the error from the server.
        /// </summary>
        public String ErrorMessage { get; set; }
    }
}
