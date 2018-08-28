using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Config
{
    public class SiteBuilderEventArgs
    {
        /// <summary>
        /// The site builder to customize.
        /// </summary>
        public ISiteBuilder SiteBuilder { get; set; }

        /// <summary>
        /// The service provider.
        /// </summary>
        public IServiceProvider Services { get; set; }
    }
}
