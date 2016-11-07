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
using LibGit2Sharp;
using EdityMcEditface.Models.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Microsoft.AspNetCore.Http;
using EdityMcEditface.Models.Auth;
using EdityMcEditface.Models.Page;
using EdityMcEditface.Models.Config;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;
using Swashbuckle.Swagger.Model;

namespace EdityMcEditface
{
    public class Startup
    {
        public static String EditySettingsRoot { get; set; }
        public static String EditySettingsFile { get; set; } = "Config/edityserver.json";

        private Info info = new Info
        {
            Version = "v1",
            Title = "Edity McEdiface API",
            Description = "The API for Edity McEdiface",
            TermsOfService = "None"
        };

        private String runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private IHostingEnvironment env;

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
                    { "EditySettings:UsersFile", Path.Combine(siteRootPath, "Config/users.json") },
                    { "EditySettings:DetailedErrors", env.IsEnvironment("Development").ToString() },
                    { "EditySettings:SecureCookies", (!env.IsEnvironment("Development")).ToString() }
                })
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            EditySettings = new EditySettings();
            ConfigurationBinder.Bind(Configuration.GetSection("EditySettings"), EditySettings);


            if (EditySettingsRoot == null)
            {
                EditySettingsRoot = siteRootPath;
                if (EditySettings.ReadFromCurrentDirectory)
                {
                    EditySettingsRoot = Path.Combine(Directory.GetCurrentDirectory());
                }
            }

            var defaultProjectPath = Directory.GetCurrentDirectory();
            //Check to see if this is our running folder, if so go into wwwroot
            if (String.Compare(defaultProjectPath, siteRootPath.TrimEnd('/', '\\')) == 0)
            {
                defaultProjectPath = Path.Combine(defaultProjectPath, "wwwroot");
            }

            builder = new ConfigurationBuilder()
            .SetBasePath(EditySettingsRoot)
            .AddInMemoryCollection(new Dictionary<String, String>
            {
                    { "ProjectMode", "SingleRepo" },
                    { "Compiler", "Direct" },
                    { "OutputPath", Path.Combine(Directory.GetCurrentDirectory(), $"..\\{Path.GetFileName(Directory.GetCurrentDirectory())}-EdityOutput") },
                    { "SiteName", "" },
                    { "ProjectPath", defaultProjectPath },
                    { "BackupFilePath", Path.Combine(siteRootPath, "wwwroot") }
            })
            .AddJsonFile(EditySettingsFile, optional: true, reloadOnChange: true)
            .AddJsonFile($"{Path.GetFileNameWithoutExtension(EditySettingsFile)}.{env.EnvironmentName}.json", optional: true);

            EdityServerConfiguration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public EditySettings EditySettings { get; set; }

        public IConfigurationRoot EdityServerConfiguration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<WorkQueue, WorkQueue>();

            services.AddScoped<AuthUserInfo>();

            switch (EdityServerConfiguration["ProjectMode"])
            {
                case "SingleRepo":
                default:
                    services.AddTransient<ProjectFinder, OneRepo>(s =>
                    {
                        return new OneRepo(EdityServerConfiguration["ProjectPath"], EdityServerConfiguration["BackupFilePath"]);
                    });
                    break;
                case "OneRepoPerUser":
                    services.AddTransient<ProjectFinder, OneRepoPerUser>(s =>
                    {
                        return new OneRepoPerUser(EdityServerConfiguration["ProjectPath"], EdityServerConfiguration["BackupFilePath"]);
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

            switch (EdityServerConfiguration["Compiler"])
            {
                case "RoundRobin":
                    services.AddTransient<SiteBuilder, RoundRobinSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var settings = createSiteBuilderSettings(s);
                        var builder = new RoundRobinSiteBuilder(settings, new WebConfigRoundRobinDeployer());

                        if (EdityServerConfiguration["ProjectMode"] == "OneRepoPerUser")
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

                        if (EdityServerConfiguration["ProjectMode"] == "OneRepoPerUser")
                        {
                            builder.addPreBuildTask(new PullPublish(projectFinder.MasterRepoPath, projectFinder.PublishedProjectPath));
                        }

                        return builder;
                    });
                    break;
            }

            // Add framework services.
            services.AddMvc(o =>
            {
                o.UseExceptionErrorFilters(EditySettings.DetailedErrors);
            })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            if (env.IsEnvironment("Development"))
            {
                services.AddConventionalSwagger(info);
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
                app.UseConventionalSwagger(info);
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = "/edity/auth/login",
                LogoutPath = "/edity/auth/logout"
            });

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
        }

        private SiteBuilderSettings createSiteBuilderSettings(IServiceProvider s)
        {
            var projectFinder = s.GetRequiredService<ProjectFinder>();

            return new SiteBuilderSettings()
            {
                InDir = projectFinder.PublishedProjectPath,
                BackupPath = projectFinder.BackupPath,
                OutDir = EdityServerConfiguration["OutputPath"],
                SiteName = EdityServerConfiguration["SiteName"]
            };
        }
    }
}
