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

namespace EdityMcEditface
{
    public class Startup
    {
        /// <summary>
        /// The root directory to look for a project.
        /// </summary>
        public static String EdityRoot { get; set; }

        /// <summary>
        /// The file for project settings shared across all users for an edity project.
        /// </summary>
        public static String EdityProjectSettingsFile { get; set; } = "Config/edityserver.json";

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

            var builder = new ConfigurationBuilder()
                .SetBasePath(siteRootPath)
                .AddInMemoryCollection(new Dictionary<String, String>
                {
                    { "EditySettings:ReadFromCurrentDirectory", "false" },
                    { "EditySettings:NoAuth", "false" },
                    { "EditySettings:NoAuthUser", "OnlyUser" },
                    { "EditySettings:DetailedErrors", env.IsEnvironment("Development").ToString() },
                })
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            EditySettings = new EditySettings();
            ConfigurationBinder.Bind(Configuration.GetSection("EditySettings"), EditySettings);

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
                    args.SiteBuilder.AddPostBuildTask(new SimpleWebConfigTask(args.SiteBuilder, ProjectConfiguration.DefaultPage));
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

            var defaultProjectPath = Directory.GetCurrentDirectory();
            //Check to see if this is our running folder, if so go into wwwroot
            if (String.Compare(defaultProjectPath, siteRootPath.TrimEnd('/', '\\')) == 0)
            {
                defaultProjectPath = Path.Combine(defaultProjectPath, "wwwroot");
            }

            builder = new ConfigurationBuilder()
            .SetBasePath(EdityRoot)
            .AddInMemoryCollection(new Dictionary<String, String>
            {
                { "ProjectMode", "SingleRepo" },
                { "Compiler", "Direct" },
                { "OutputPath", Path.Combine(Directory.GetCurrentDirectory(), $"..\\{Path.GetFileName(Directory.GetCurrentDirectory())}-EdityOutput") },
                { "SiteName", "" },
                { "ProjectPath", defaultProjectPath },
                { "EdityCorePath", Path.Combine(siteRootPath, "ClientBin/EdityMcEditface") },
                { "SitePath", Path.Combine(siteRootPath, "ClientBin/Site") }
            })
            .AddJsonFile(EdityProjectSettingsFile, optional: true, reloadOnChange: true)
            .AddJsonFile($"{Path.GetFileNameWithoutExtension(EdityProjectSettingsFile)}.{env.EnvironmentName}.json", optional: true);

            var edityProjectConfiguration = builder.Build();
            ConfigurationBinder.Bind(edityProjectConfiguration, ProjectConfiguration);
        }

        public IConfigurationRoot Configuration { get; }

        public EditySettings EditySettings { get; private set; }

        public ProjectConfiguration ProjectConfiguration { get; private set; } = new ProjectConfiguration();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEdity(EditySettings, ProjectConfiguration);

            services.AddAuthentication(AuthCoreSchemes.Bearer)
            .AddCookie(AuthCoreSchemes.Bearer, o =>
            {
                o.LoginPath = "/edity/auth/login";
                o.LogoutPath = "/edity/auth/logout";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

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

            app.UseEdity();
        }
    }
}
