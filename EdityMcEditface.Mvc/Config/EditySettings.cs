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
        /// The user account name to use when using NoAuth
        /// </summary>
        public String NoAuthUser { get; set; } = "OnlyUser";

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
    }
}
