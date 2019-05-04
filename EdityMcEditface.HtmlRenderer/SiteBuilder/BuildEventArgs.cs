using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class BuildEventArgs
    {
        public BuildEventArgs(IBuildStatusTracker tracker, ISiteBuilder siteBuilder, BuilderUserInfo userInfo)
        {
            this.Tracker = tracker;
            this.SiteBuilder = siteBuilder;
            this.UserInfo = userInfo;
        }

        public IBuildStatusTracker Tracker { get; private set; }

        public ISiteBuilder SiteBuilder { get; private set; }

        public BuilderUserInfo UserInfo { get; private set; }
    }
}
