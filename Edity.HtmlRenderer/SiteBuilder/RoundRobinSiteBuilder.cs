using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class RoundRobinSiteBuilder : SiteBuilder
    {
        private SiteBuilderSettings settings;
        private DirectOutputSiteBuilder directOutput;
        private String baseOutputFolder;
        private String outputFullPath;
        private RoundRobinDeployer deployer;

        public RoundRobinSiteBuilder(SiteBuilderSettings settings, RoundRobinDeployer deployer)
        {
            this.deployer = deployer;
            baseOutputFolder = Path.GetFullPath(settings.OutDir);
            outputFullPath = Path.GetFullPath(Path.Combine(baseOutputFolder, Guid.NewGuid().ToString()));
            settings.OutDir = outputFullPath;
            this.settings = settings;
            directOutput = new DirectOutputSiteBuilder(settings);
        }

        public void BuildSite()
        {
            directOutput.BuildSite();

            deployer.Deploy(settings.OutDir);

            //Delete old virtual directories
            foreach(var dir in Directory.EnumerateDirectories(baseOutputFolder))
            {
                if(dir != outputFullPath)
                {
                    try
                    {
                        DirectOutputSiteBuilder.MultiTryDirDelete(dir);
                    }
                    catch (Exception) { }
                }
            }
        }

        public void addPreBuildTask(BuildTask task)
        {
            directOutput.addPreBuildTask(task);
        }

        public void addPostBuildTask(BuildTask task)
        {
            directOutput.addPostBuildTask(task);
        }
    }
}
