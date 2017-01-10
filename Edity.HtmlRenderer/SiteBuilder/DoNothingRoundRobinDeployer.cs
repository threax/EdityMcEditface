using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class DoNothingRoundRobinDeployer : RoundRobinDeployer
    {
        public DoNothingRoundRobinDeployer()
        {
            
        }

        public bool Deploy(String outputFolder)
        {
            return true;
        }
    }
}
