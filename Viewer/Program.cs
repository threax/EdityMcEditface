using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 9000;
            //String rootDir = args.Length > 0 ? args[0] : ".";
            if(args.Length > 0)
            {
                Environment.CurrentDirectory = Path.GetFullPath(args[0]);
            }
            if(args.Length > 1)
            {
                string path = "";
                path = args[1];

                try
                {
                    Process.Start($"http://localhost:{port}/{path}");
                }
                catch (Exception e)
                {

                }
            }

            var qInfo = new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false);
            var bInfo = new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false);

            using (OwinMicroSite site = new OwinMicroSite(port))
            {
                Console.WriteLine($"Running site on {Environment.CurrentDirectory}");
                Console.WriteLine("Press b to open your browser");
                Console.WriteLine("Press q to quit.");

                while (true)
                {
                    var key = Console.ReadKey();
                    if(key == qInfo)
                    {
                        break;
                    }
                    if(key == bInfo)
                    {
                        Process.Start($"http://localhost:{port}");
                    }
                }
            }
        }
    }
}
