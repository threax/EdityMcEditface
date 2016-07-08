using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class RoundRobinSiteBuilder : SiteBuilder
    {
        private SiteBuilderSettings settings;
        private DirectOutputSiteBuilder directOutput;

        public RoundRobinSiteBuilder(SiteBuilderSettings settings)
        {
            settings.OutDir = Path.Combine(settings.OutDir, Guid.NewGuid().ToString());
            this.settings = settings;
            directOutput = new DirectOutputSiteBuilder(settings);
        }

        public async Task BuildSite()
        {
            await directOutput.BuildSite();
        }
    }
}
