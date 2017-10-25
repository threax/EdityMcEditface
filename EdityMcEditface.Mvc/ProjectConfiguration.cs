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
        /// The path to the edity core client side files.
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
        /// The override variables for the project. These variables take ultimate precidence over all other
        /// variables defined in edity.json files. Mostly useful to override variables when working on the code.
        /// Defaults to null, which means no overrides.
        /// </summary>
        public Dictionary<String, String> OverrideVars { get; set; } = null;
    }
}
