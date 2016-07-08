using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface RoundRobinDeployer
    {
        /// <summary>
        /// Do whatever deployment steps are needed for this site.
        /// </summary>
        /// <returns></returns>
        bool Deploy(String outputFolder);
    }
}
