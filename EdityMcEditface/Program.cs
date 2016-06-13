using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EdityMcEditface.NetCore.Controllers;

namespace EdityMcEditface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            HomeController.BackupFileSource = Path.Combine(runningFolder, "../../../wwwroot");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
