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

        public RoundRobinSiteBuilder(SiteBuilderSettings settings)
        {
            baseOutputFolder = settings.OutDir;
            outputFullPath = Path.GetFullPath(Path.Combine(baseOutputFolder, Guid.NewGuid().ToString()));
            settings.OutDir = outputFullPath;
            this.settings = settings;
            directOutput = new DirectOutputSiteBuilder(settings);
        }

        public async Task BuildSite()
        {
            await directOutput.BuildSite();

            //Swap virtual directories
            //This method might need admin access
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "appcmd",
                Arguments = $"set vdir /vdir.name:{settings.CompiledVirtualFolder} /physicalPath:{settings.OutDir}"
            });

            process.WaitForExit();

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
    }
}
