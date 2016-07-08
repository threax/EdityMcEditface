using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class AppCmdRoundRobinDeployer : RoundRobinDeployer
    {
        private String virtualFolder;

        public AppCmdRoundRobinDeployer(String virtualFolder)
        {
            this.virtualFolder = virtualFolder;
        }

        public bool Deploy(String outputFolder)
        {
            //Swap virtual directories
            //This method might need admin access
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "appcmd",
                Arguments = $"set vdir /vdir.name:{virtualFolder} /physicalPath:{outputFolder}"
            });

            process.WaitForExit();

            return process.ExitCode == 0;
        }
    }
}
