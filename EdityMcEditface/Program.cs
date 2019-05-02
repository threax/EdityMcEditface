using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EdityMcEditface.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Threax.AspNetCore.BuiltInTools;

namespace EdityMcEditface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tools = new ToolManager(args);
            var toolsEnv = tools.GetEnvironment();
            var toolsConfigName = default(String);
            if (toolsEnv != null)
            {
                //If we are running tools, clear the arguments (this causes an error if the tool args are passed) and set the tools config to the environment name
                args = tools.GetCleanArgs();
                toolsConfigName = toolsEnv;
            }

#if LOCAL_RUN_ENABLED
            var commandLineConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
#endif

            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

#if LOCAL_RUN_ENABLED
            webHostBuilder.UseConfiguration(commandLineConfig);

            var browseUrl = commandLineConfig["browse"];
            if (!String.IsNullOrEmpty(browseUrl))
            {
                String hostUrl = "http://localhost:" + FreeTcpPort();
                webHostBuilder.UseUrls(hostUrl);
                var uri = new Uri(new Uri(hostUrl), browseUrl);
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    Thread.Sleep(200);
                    Process.Start(uri.ToString());
                });
            }

            var workingDirPath = commandLineConfig["workingDir"];
            if (!String.IsNullOrEmpty(workingDirPath))
            {
                var fullWorkDir = Path.GetFullPath(workingDirPath);
                Directory.SetCurrentDirectory(fullWorkDir);
            }
#endif

            var host = webHostBuilder.Build();

            if (tools.ProcessTools(host))
            {
                host.Run();
            }
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
