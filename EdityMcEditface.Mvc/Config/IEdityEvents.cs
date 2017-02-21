using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Config
{
    public interface IEdityEvents
    {
        /// <summary>
        /// Called when a sitebuilder is created, extra events can be added to it here.
        /// </summary>
        /// <param name="args">The args for the event.</param>
        void CustomizeSiteBuilder(SiteBuilderEventArgs args);
    }
}
