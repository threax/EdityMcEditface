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
using System.Runtime.InteropServices;

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

            var commandLineConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

            webHostBuilder.UseConfiguration(commandLineConfig);

            String hostUrl = "http://localhost:" + FreeTcpPort();
            webHostBuilder.UseUrls(hostUrl);

            var workingDirPath = commandLineConfig["workingDir"];
            if (!String.IsNullOrEmpty(workingDirPath))
            {
                var fullWorkDir = Path.GetFullPath(workingDirPath);
                Directory.SetCurrentDirectory(fullWorkDir);
            }

            var host = webHostBuilder.Build();

            if (tools.ProcessTools(host))
            {
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    try
                    {
                        OpenBrowser(hostUrl);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Cannot start browser: {ex.Message}");
                    }
                });

                host.Run();
            }
        }

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

        public static Process OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")); // Works ok on windows
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Process.Start("xdg-open", url);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Process.Start("open", url); // Not tested
            }

            throw new InvalidOperationException("Cannot open browser, unknown OS");
        }
    }
}
