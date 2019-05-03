using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Config
{
    /// <summary>
    /// The settings class.
    /// </summary>
    public class EditySettings
    {
        /// <summary>
        /// True to read only from the current directory.
        /// </summary>
        public bool ReadFromCurrentDirectory { get; set; }

        /// <summary>
        /// True to get detailed errors.
        /// </summary>
        public bool DetailedErrors { get; set; }

        /// <summary>
        /// Some events that are fired during runtime.
        /// </summary>
        [JsonIgnore]
        public IEdityEvents Events { get; set; } = new EdityEvents();

        /// <summary>
        /// Set this to an enumerable of assemblies to add them as application parts to mvc.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Assembly> AdditionalMvcLibraries { get; set; }

        /// <summary>
        /// The base url for the website, can be null unless the site is hosted in a subfolder, then you
        /// should set this to the subfolder path.
        /// </summary>
        public String BaseUrl { get; set; }

        /// <summary>
        /// The mode to handle the project's files.
        /// </summary>
        public ProjectMode ProjectMode { get; set; } = ProjectMode.OneRepo;

        /// <summary>
        /// The publisher to use.
        /// </summary>
        public Publishers Publisher { get; set; } = Publishers.Direct;

        /// <summary>
        /// The output folder for published files.
        /// </summary>
        public String OutputPath { get; set; }

        /// <summary>
        /// The path to the project.
        /// </summary>
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
        /// The override variables for the project. These variables take ultimate precidence over all other
        /// variables defined in edity.json files. Mostly useful to override variables when working on the code.
        /// Defaults to null, which means no overrides.
        /// </summary>
        public Dictionary<String, String> OverrideVars { get; set; } = null;
    }
}
