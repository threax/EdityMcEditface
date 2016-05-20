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
            string path = "";
            if(args.Length > 0)
            {
                path = args[0];
            }
            try
            {
                Process.Start($"http://localhost:{port}/{path}");
            }
            catch (Exception e)
            {
                
            }

            using (OwinMicroSite site = new OwinMicroSite(port))
            {
                site.run();
            }
        }
    }
}
