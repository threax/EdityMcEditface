using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc
{
    public class ProjectConfiguration
    {
        public String ProjectMode { get; set; }

        public String Compiler { get; set; }

        public String OutputPath { get; set; }

        public String SiteName { get; set; }

        public String ProjectPath { get; set; }

        /// <summary>
        /// The path to th edity core client side files.
        /// </summary>
        public String EdityCorePath { get; set; }

        /// <summary>
        /// The path to this site's client side files.
        /// </summary>
        public String SitePath { get; set; }

        /// <summary>
        /// The default page for the project. Defaults to "index".
        /// </summary>
        public String DefaultPage { get; set; } = "index";

        /// <summary>
        /// The name of the live branch, the live branch gets published when the site compiles, defaults to "live"
        /// </summary>
        public String LiveBranchName { get; set; } = "live";
    }
}
