using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using EdityMcEditface.Mvc.Config;
using EdityMcEditface.Mvc;
using EdityMcEditface.HtmlRenderer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Authorization;
using EdityMcEditface.BuildTasks;
using Threax.AspNetCore.BuiltInTools;
using EdityMcEditface.ToolControllers;
using Threax.Extensions.Configuration.SchemaBinder;

namespace EdityMcEditface
{
    public class Startup
    {
        /// <summary>
        /// The root directory to look for a project.
        /// </summary>
        public static String EdityRoot { get; set; }

        private String runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

            String siteRootPath = runningFolder;
            if (env.IsEnvironment("Development"))
            {
                if (!Directory.Exists(Path.Combine(siteRootPath, "ClientBin")))
                {
                    //Probably running inside the output folder, go up the appropriate number of directories
                    siteRootPath = Path.GetFullPath(Path.Combine(runningFolder, "../../../"));
                    if (!Directory.Exists(Path.Combine(siteRootPath, "ClientBin")))
                    {
                        throw new Exception("Cannot find site root folder containing a ClientBin folder");
                    }
                }
            }

            var defaultProjectPath = Directory.GetCurrentDirectory();
            //Check to see if this is our running folder, if so go into wwwroot
            if (String.Compare(defaultProjectPath, siteRootPath.TrimEnd('/', '\\')) == 0)
            {
                defaultProjectPath = Path.Combine(defaultProjectPath, "wwwroot");
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(siteRootPath)
                .AddInMemoryCollection(new Dictionary<String, String>
                {
                    { "EditySettings:ReadFromCurrentDirectory", "false" },
                    { "EditySettings:NoAuth", "false" },
                    { "EditySettings:NoAuthUser", "OnlyUser" },
                    { "EditySettings:DetailedErrors", env.IsEnvironment("Development").ToString() },
                    { "EdityProject:ProjectMode", "OneRepo" },
                    { "EdityProject:OutputPath", Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"..\\{Path.GetFileName(Directory.GetCurrentDirectory())}-EdityOutput")) },
                    { "EdityProject:ProjectPath", defaultProjectPath },
                    { "EdityProject:EdityCorePath", Path.Combine(siteRootPath, "ClientBin/EdityMcEditface") },
                    { "EdityProject:SitePath", Path.Combine(siteRootPath, "ClientBin/Site") }
                })
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = new SchemaConfigurationBinder(builder.Build());

            EditySettings = new EditySettings();
            Configuration.Bind("EditySettings", EditySettings);
            Configuration.Bind("EdityProject", ProjectConfiguration);

            //Setup the menu publish task, this is done per project or configuration instead of in the core.
            EditySettings.Events = new EdityEvents()
            {
                OnCustomizeSiteBuilder = args =>
                {
                    var fileFinder = args.Services.GetRequiredService<IFileFinder>();
                    var serializer = new JsonSerializer()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    };
                    serializer.Converters.Add(new StringEnumConverter());
                    args.SiteBuilder.AddPostBuildTask(new PublishMenu(fileFinder, args.SiteBuilder, "menus/mainMenu.json", serializer));
                    switch (ProjectConfiguration.Publisher)
                    {
                        case Publishers.AzureZip:
                            args.SiteBuilder.AddPostBuildTask(new AddAzureRoundRobinScripts(args.SiteBuilder, ProjectConfiguration.DefaultPage));
                            break;
                        case Publishers.Direct:
                            args.SiteBuilder.AddPostBuildTask(new SimpleWebConfigTask(args.SiteBuilder, ProjectConfiguration.DefaultPage));
                            break;
                    }
                }
            };


            if (EdityRoot == null)
            {
                EdityRoot = siteRootPath;
                if (EditySettings.ReadFromCurrentDirectory)
                {
                    EdityRoot = Path.Combine(Directory.GetCurrentDirectory());
                }
            }
        }

        public SchemaConfigurationBinder Configuration { get; }

        public EditySettings EditySettings { get; private set; }

        public ProjectConfiguration ProjectConfiguration { get; private set; } = new ProjectConfiguration();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEdity(EditySettings, ProjectConfiguration, o =>
            {
                o.CustomizeTools = tools =>
                {
                    tools
                    .AddTool("updateConfigSchema", new ToolCommand("Update the schema file for this application's configuration.", async a =>
                    {
                        var json = await Configuration.CreateSchema();
                        File.WriteAllText("appsettings.schema.json", json);
                    }))
                    .AddTool("clone", new ToolCommand("Clone a git repo.", a => RepoTools.Clone(a, ProjectConfiguration)));

                    if (ProjectConfiguration.ProjectMode == ProjectMode.OneRepoPerUser)
                    {
                        tools.AddTool("pushmaster", new ToolCommand("Push a one repo per user shared repo to an origin. Only works in OneRepoPerUser mode.", a => RepoTools.PushMaster(a, ProjectConfiguration)));
                    }
                };
            });

            services.AddAuthentication(AuthCoreSchemes.Bearer)
            .AddCookie(AuthCoreSchemes.Bearer, o =>
            {
                o.LoginPath = "/edity/auth/login";
                o.LogoutPath = "/edity/auth/logout";
            });

            services.AddLogging(o =>
            {
                o.AddConfiguration(Configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseEdity();
        }
    }
}
