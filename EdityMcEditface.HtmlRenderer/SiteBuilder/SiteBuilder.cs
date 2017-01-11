using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface SiteBuilder
    {
        void addPreBuildTask(BuildTask task);

        void addPostBuildTask(BuildTask task);

        void BuildSite();
    }
}
