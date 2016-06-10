using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Web.Http;

namespace Edity.McEditface
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "Embedded",
                routeTemplate: "embd/{*file}",
                defaults: new
                {
                    controller = "EmbeddedFile",
                }
            );

            config.Routes.MapHttpRoute(
                name: "Upload",
                routeTemplate: FileController.UploadPath + "{*file}",
                defaults: new
                {
                    controller = "File",
                    action = "Post"
                }
            );

            config.Routes.MapHttpRoute(
                name: "Directory",
                routeTemplate: FileController.UploadPath + "{*file}",
                defaults: new
                {
                    controller = "File",
                    action = "File"
                }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{*file}",
                
                defaults: new
                {
                    controller = "File",
                }
            );

            var fileSystem = new PhysicalFileSystem(Environment.CurrentDirectory);
            var options = new FileServerOptions();
            
            options.EnableDirectoryBrowsing = true;
            options.FileSystem = fileSystem;
            options.StaticFileOptions.ContentTypeProvider = new CommonMarkContentTypeProvider();

            //Building in this order makes everything work
            appBuilder.UseFileServer(options);
            appBuilder.UseWebApi(config);
        }
    }
}