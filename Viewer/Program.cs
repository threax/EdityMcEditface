using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (OwinMicroSite site = new OwinMicroSite(9000))
            {
                site.run();
            }
        }
    }
}
