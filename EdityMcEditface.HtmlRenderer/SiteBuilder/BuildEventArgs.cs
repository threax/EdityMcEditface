using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class BuildEventArgs
    {
        public BuildEventArgs(IBuildStatusTracker tracker)
        {
            this.Tracker = tracker;
        }

        public IBuildStatusTracker Tracker { get; set; }
    }
}
