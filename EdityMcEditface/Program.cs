#define LOCAL_RUN_ENABLED

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EdityMcEditface.NetCore.Controllers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace EdityMcEditface
{
    public class Program
    {
        public static void Main(string[] args)
        {

#if LOCAL_RUN_ENABLED
            var commandLineConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
#endif

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

#if LOCAL_RUN_ENABLED
            host.UseConfiguration(commandLineConfig);

            var browseUrl = commandLineConfig["browse"];
            if (!String.IsNullOrEmpty(browseUrl))
            {
                String hostUrl = "http://localhost:" + FreeTcpPort();
                host.UseUrls(hostUrl);
                var uri = new Uri(new Uri(hostUrl), browseUrl);
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    Thread.Sleep(200);
                    Process.Start(uri.ToString());
                });
            }

            var configPath = commandLineConfig["config"];
            if (!String.IsNullOrEmpty(configPath))
            {
                configPath = Path.GetFullPath(configPath);
                if (File.Exists(configPath))
                {
                    Startup.EditySettingsFile = Path.GetFileName(configPath);
                    Startup.EditySettingsRoot = Path.GetDirectoryName(configPath);
                }
            }

            var workingDirPath = commandLineConfig["workingDir"];
            if (!String.IsNullOrEmpty(workingDirPath))
            {
                var fullWorkDir = Path.GetFullPath(workingDirPath);
                Environment.CurrentDirectory = fullWorkDir;
            }
#endif

            host.Build().Run();
        }

#if LOCAL_RUN_ENABLED
        //This has race conditions, but only used when the browser is being opened from the command line
        //which should basically always work in practice, dont call this for any other reason
        static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
#endif
    }
}
