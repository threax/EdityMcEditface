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
            var commandLineConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(commandLineConfig)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

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

            host.Build().Run();
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
    }
}
