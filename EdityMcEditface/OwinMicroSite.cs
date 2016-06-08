using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Edity.McEditface
{
    class OwinMicroSite : IDisposable
    {
        private ManualResetEventSlim siteWake;
        private IDisposable site;
        private int port;

        public OwinMicroSite(int port)
        {
            this.port = port;
            siteWake = new ManualResetEventSlim(false);
            // Start OWIN host 
            site = WebApp.Start<Startup>(url: $"http://localhost:{port}");
        }

        public void Dispose()
        {
            site.Dispose();
            siteWake.Dispose();
        }

        public int Port
        {
            get
            {
                return port;
            }
        }

        public void run()
        {
            siteWake.Wait();
        }

        public void exit()
        {
            siteWake.Set();
        }
    }
}
