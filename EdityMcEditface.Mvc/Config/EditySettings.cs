using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// This event is fired when the site builder is created, it can be customized further in this callback.
        /// </summary>
        public event Action<SiteBuilder> CustomizeSiteBuilder;

        /// <summary>
        /// Fire the customize site builder event.
        /// </summary>
        /// <param name="siteBuilder"></param>
        public void FireCustomizeSiteBuilder(SiteBuilder siteBuilder)
        {
            if(this.CustomizeSiteBuilder != null)
            {
                this.CustomizeSiteBuilder.Invoke(siteBuilder);
            }
        }
    }
}
