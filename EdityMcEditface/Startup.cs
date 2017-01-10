using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EdityMcEditface.HtmlRenderer;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using LibGit2Sharp;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Microsoft.AspNetCore.Http;
using EdityMcEditface.Models.Page;
using Swashbuckle.Swagger.Model;
using Edity.PluginCore;
using System.Reflection;
using Edity.PluginCore.Config;

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

        private Info info = new Info
        {
            Version = "v1",
            Title = "Edity McEdiface API",
            Description = "The API for Edity McEdiface",
            TermsOfService = "None"
        };

        private String runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private IHostingEnvironment env;
        private List<IEdityPlugin> plugins = new List<IEdityPlugin>();

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

            String siteRootPath = runningFolder;
            if (env.IsEnvironment("Development"))
            {
                if (!Directory.Exists(Path.Combine(siteRootPath, "wwwroot")))
                {
                    //Probably running inside the output folder, go up the appropriate number of directories
                    siteRootPath = Path.GetFullPath(Path.Combine(runningFolder, "../../../../"));
                    if (!Directory.Exists(Path.Combine(siteRootPath, "wwwroot")))
                    {
                        throw new Exception("Cannot find site root folder containing a backup wwwroot folder");
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
                    { "BackupFilePath", Path.Combine(siteRootPath, "wwwroot") }
            })
            .AddJsonFile(EdityProjectSettingsFile, optional: true, reloadOnChange: true)
            .AddJsonFile($"{Path.GetFileNameWithoutExtension(EdityProjectSettingsFile)}.{env.EnvironmentName}.json", optional: true);

            var edityProjectConfiguration = builder.Build();
            ConfigurationBinder.Bind(edityProjectConfiguration, ProjectConfiguration);

            //Load plugin dlls from config.
            foreach (var plugin in EditySettings.Plugins)
            {
                var file = Path.Combine(runningFolder, plugin);
                var assembly = Assembly.LoadFrom(file);
                EdityPluginEntryPointAttribute entryPoint;
                foreach(var attr in assembly.GetCustomAttributes())
                {
                    entryPoint = attr as EdityPluginEntryPointAttribute;
                    if(entryPoint != null)
                    {
                        plugins.AddRange(entryPoint.CreatePlugins());
                    }
                }
            }

            //Initialize plugins.
            foreach(var plugin in plugins)
            {
                plugin.OnStartup(env, Configuration, edityProjectConfiguration);
            }
        }

        public IConfigurationRoot Configuration { get; }

        public EditySettings EditySettings { get; private set; }

        public ProjectConfiguration ProjectConfiguration { get; private set; } = new ProjectConfiguration();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var plugin in plugins)
            {
                plugin.ConfigureServicesStart(services);
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<WorkQueue, WorkQueue>();

            services.AddScoped<AuthUserInfo>();

            services.AddSingleton<EditySettings>(s => EditySettings);

            switch (ProjectConfiguration.ProjectMode)
            {
                case "SingleRepo":
                default:
                    services.AddTransient<ProjectFinder, OneRepo>(s =>
                    {
                        return new OneRepo(ProjectConfiguration.ProjectPath, ProjectConfiguration.BackupFilePath);
                    });
                    break;
                case "OneRepoPerUser":
                    services.AddTransient<ProjectFinder, OneRepoPerUser>(s =>
                    {
                        return new OneRepoPerUser(ProjectConfiguration.ProjectPath, ProjectConfiguration.BackupFilePath);
                    });
                    break;
            }

            services.AddTransient<FileFinder, FileFinder>(s =>
            {
                var userInfo = s.GetRequiredService<AuthUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.UserName);
                return new FileFinder(projectFolder, projectFinder.BackupPath);
            });

            services.AddTransient<Repository, Repository>(s =>
            {
                var userInfo = s.GetRequiredService<AuthUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.UserName);
                return new Repository(Repository.Discover(projectFolder));
            });

            services.AddTransient<Signature, Signature>(s =>
            {
                var userInfo = s.GetRequiredService<AuthUserInfo>();
                return new Signature(userInfo.UserName, userInfo.UserName + "@nowhere.com", DateTime.Now);
            });

            switch (ProjectConfiguration.Compiler)
            {
                case "RoundRobin":
                    services.AddTransient<SiteBuilder, RoundRobinSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var settings = createSiteBuilderSettings(s);
                        var builder = new RoundRobinSiteBuilder(settings, new WebConfigRoundRobinDeployer());

                        if (ProjectConfiguration.ProjectMode == "OneRepoPerUser")
                        {
                            builder.addPreBuildTask(new PullPublish(projectFinder.MasterRepoPath, projectFinder.PublishedProjectPath));
                        }

                        return builder;
                    });
                    break;
                case "Direct":
                default:
                    services.AddTransient<SiteBuilder, DirectOutputSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var builder = new DirectOutputSiteBuilder(createSiteBuilderSettings(s));

                        if (ProjectConfiguration.ProjectMode == "OneRepoPerUser")
                        {
                            builder.addPreBuildTask(new PullPublish(projectFinder.MasterRepoPath, projectFinder.PublishedProjectPath));
                        }

                        return builder;
                    });
                    break;
            }

            // Add framework services.
            var mvcBuilder = services.AddMvc(o =>
            {
                o.UseExceptionErrorFilters(EditySettings.DetailedErrors);
            })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            foreach (var plugin in plugins)
            {
                plugin.ConfigureMvc(mvcBuilder);
            }

            if (env.IsEnvironment("Development"))
            {
                services.AddConventionalSwagger(info);
            }

            foreach (var plugin in plugins)
            {
                plugin.ConfigureServicesEnd(services);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            foreach (var plugin in plugins)
            {
                plugin.ConfigureStart(app, env, loggerFactory);
            }

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
                app.UseConventionalSwagger(info);
            }

            app.UseMvc(routes =>
            {
#if LOCAL_RUN_ENABLED
                if (EditySettings.NoAuth)
                {
                    routes.MapRoute(
                        name: "NoAuthAuth",
                        template: "edity/auth/{action}",
                        defaults: new { controller = "NoAuth" }
                    );
                }
#endif

                routes.MapRoute(
                    name: "default",
                    template: "{*file}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });

            foreach (var plugin in plugins)
            {
                plugin.ConfigureEnd(app, env, loggerFactory);
            }
        }

        private SiteBuilderSettings createSiteBuilderSettings(IServiceProvider s)
        {
            var projectFinder = s.GetRequiredService<ProjectFinder>();

            return new SiteBuilderSettings()
            {
                InDir = projectFinder.PublishedProjectPath,
                BackupPath = projectFinder.BackupPath,
                OutDir = ProjectConfiguration.OutputPath,
                SiteName = ProjectConfiguration.SiteName
            };
        }
    }
}
