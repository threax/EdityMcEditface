using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EdityMcEditface.NetCore.Controllers;
using EdityMcEditface.HtmlRenderer;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace EdityMcEditface
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<FileFinder, FileFinder>(serviceProvider =>
            {
                var projectFolder = Configuration["EditySettings:ProjectPath"];
                var runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                runningFolder = Path.Combine(runningFolder, Configuration["EditySettings:BackupFilePath"]);
                return new FileFinder(projectFolder, runningFolder);
            });

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = new EdityContentTypeProvider()
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{*file}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });
        }
    }
}
