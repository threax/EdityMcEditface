using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            String rootDir = args.Length > 0 ? args[0] : ".";
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

            using (OwinMicroSite site = new OwinMicroSite(port, rootDir, new CommonMarkContentTypeProvider()))
            {
                site.run();
            }
        }
    }
}
