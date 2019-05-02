using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class BuildEventArgs
    {
        public BuildEventArgs(IBuildStatusTracker tracker, ISiteBuilder siteBuilder)
        {
            this.Tracker = tracker;
            this.SiteBuilder = siteBuilder;
        }

        public IBuildStatusTracker Tracker { get; private set; }

        public ISiteBuilder SiteBuilder { get; private set; }
    }
}
