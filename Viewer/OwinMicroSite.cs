using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.StaticFiles.ContentTypes;

namespace Viewer
{
    class OwinMicroSite : IDisposable
    {
        private ManualResetEventSlim siteWake;
        private IDisposable site;
        private int port;

        public OwinMicroSite(int port, String root, IContentTypeProvider contentTypeProvider)
        {
            var fileSystem = new PhysicalFileSystem(root);
            var options = new FileServerOptions();

            options.EnableDirectoryBrowsing = true;
            options.FileSystem = fileSystem;
            options.StaticFileOptions.ContentTypeProvider = contentTypeProvider;

            this.port = port;
            siteWake = new ManualResetEventSlim(false);
            // Start OWIN host 
            site = WebApp.Start($"http://localhost:{port}", builder => builder.UseFileServer(options));
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
