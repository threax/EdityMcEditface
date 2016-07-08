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
using Newtonsoft.Json.Converters;
using Swashbuckle.SwaggerGen.Generator;
using LibGit2Sharp;
using EdityMcEditface.Models.Compiler;
using EdityMcEditface.ErrorHandling;
using EdityMcEditface.HtmlRenderer.SiteBuilder;

namespace EdityMcEditface
{
    public class Startup
    {
        private String runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

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
                var backupPath = Path.Combine(runningFolder, Configuration["EditySettings:BackupFilePath"]);
                return new FileFinder(projectFolder, backupPath);
            });

            services.AddTransient<Repository, Repository>(s =>
            {
                var projectFolder = Configuration["EditySettings:ProjectPath"];
                return new Repository(projectFolder);
            });

            switch (Configuration["EditySettings:Compiler"])
            {
                case "RoundRobin":
                    services.AddTransient<SiteBuilder, RoundRobinSiteBuilder>(s =>
                    {
                        var settings = createSiteBuilderSettings();
                        //return new RoundRobinSiteBuilder(settings, new AppCmdRoundRobinDeployer(settings.CompiledVirtualFolder));
                        return new RoundRobinSiteBuilder(settings, new ServerManagerRoundRobinDeployer(settings.SiteName, settings.CompiledVirtualFolder));
                    });
                    break;
                default:
                    services.AddTransient<SiteBuilder, DirectOutputSiteBuilder>(s =>
                    {
                        return new DirectOutputSiteBuilder(createSiteBuilderSettings());
                    });
                    break;
            }

            // Add framework services.
            services.AddMvc(o =>
            {
                o.Filters.Add(new ExceptionToJsonFilterAttribute(env.IsEnvironment("Development")));
            })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            if (env.IsEnvironment("Development"))
            {
                services.AddSwaggerGen();
                services.ConfigureSwaggerGen(options =>
                {
                    options.SingleApiVersion(new Info
                    {
                        Version = "v1",
                        Title = "Edity McEdiface API",
                        Description = "The API for Edity McEdiface",
                        TermsOfService = "None"
                    });
                    string pathToDoc = Path.Combine(runningFolder, "EdityMcEditface.xml");
                    if (File.Exists(pathToDoc))
                    {
                        options.IncludeXmlComments(pathToDoc);
                    }
                    options.DescribeAllEnumsAsStrings();
                });
            }
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

            if (env.IsEnvironment("Development"))
            {
                app.UseSwaggerGen();
                app.UseSwaggerUi();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{*file}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });
        }

        private SiteBuilderSettings createSiteBuilderSettings()
        {
            return new SiteBuilderSettings()
            {
                InDir = Configuration["EditySettings:ProjectPath"],
                BackupPath = Path.Combine(runningFolder, Configuration["EditySettings:BackupFilePath"]),
                OutDir = Configuration["EditySettings:OutputPath"],
                CompiledVirtualFolder = Configuration["EditySettings:CompiledVirtualFolder"],
                SiteName = Configuration["EditySettings:SiteName"]
            };
        }
    }
}
