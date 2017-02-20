using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc
{
    public class WebConfigRoundRobinDeployer : RoundRobinDeployer
    {
        private IWebConfigProvider webConfigProvider;

        public WebConfigRoundRobinDeployer(IWebConfigProvider webConfigProvider)
        {
            this.webConfigProvider = webConfigProvider;
        }

        public bool Deploy(string outputFolder)
        {
            var innerFolder = Path.GetFileName(outputFolder);
            var outputPath = Path.GetDirectoryName(outputFolder);

            var webConfigFile = Path.Combine(outputPath, "web.config");
            var webConfigOutput = String.Format(webConfigProvider.WebConfigTemplate, innerFolder);

            using (var writer = new StreamWriter(File.Open(webConfigFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(webConfigOutput);
            }

            return true;
        }
    }
}
