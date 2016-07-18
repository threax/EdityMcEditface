﻿using System;
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
using Microsoft.AspNetCore.Http;
using EdityMcEditface.Models.Auth;
using EdityMcEditface.Models.Page;

namespace EdityMcEditface
{
    public class Startup
    {
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
                    { "EditySettings:ReadFromCurrentDirectory", "false" }
                })
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            String editySettingsRoot = siteRootPath;
            bool readSettingsFromCurrent;
            if (!bool.TryParse(Configuration["EditySettings:ReadFromCurrentDirectory"], out readSettingsFromCurrent))
            {
                readSettingsFromCurrent = false;
            }
            if (readSettingsFromCurrent)
            {
                editySettingsRoot = Path.Combine(Directory.GetCurrentDirectory());
            }

            builder = new ConfigurationBuilder()
            .SetBasePath(editySettingsRoot)
            .AddInMemoryCollection(new Dictionary<String, String>
            {
                    { "ProjectMode", "SingleRepo" },
                    { "Compiler", "Direct" },
                    { "OutputPath", Path.Combine(Directory.GetCurrentDirectory(), "Compiled") },
                    { "CompiledVirtualFolder", "" },
                    { "SiteName", "" },
                    { "ProjectPath", Directory.GetCurrentDirectory() },
                    { "BackupFilePath", Path.Combine(siteRootPath, "wwwroot") }
            })
            .AddJsonFile("edityserver.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"edityserver.{env.EnvironmentName}.json", optional: true);

            EdityServerConfiguration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IConfigurationRoot EdityServerConfiguration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.User);
                return new FileFinder(projectFolder, projectFinder.BackupPath);
            });

            services.AddTransient<Repository, Repository>(s =>
            {
                var userInfo = s.GetRequiredService<AuthUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.User);
                return new Repository(projectFolder);
            });

            services.AddTransient<AuthChecker, AuthChecker>();

            switch (EdityServerConfiguration["Compiler"])
            {
                case "RoundRobin":
                    services.AddTransient<SiteBuilder, RoundRobinSiteBuilder>(s =>
                    {
                        var settings = createSiteBuilderSettings(s);
                        return new RoundRobinSiteBuilder(settings, new ServerManagerRoundRobinDeployer(settings.SiteName, settings.CompiledVirtualFolder)
                        {
                            AppHostConfigPath = EdityServerConfiguration["AppHostConfigPath"]
                        });
                    });
                    break;
                case "Direct":
                default:
                    services.AddTransient<SiteBuilder, DirectOutputSiteBuilder>(s =>
                    {
                        return new DirectOutputSiteBuilder(createSiteBuilderSettings(s));
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = AuthenticationConfig.CookieAuthenticationSchemeName,
                LoginPath = new PathString("/edity/Auth/LogIn/"),
                AccessDeniedPath = new PathString("/edity/Auth/AccessDenied/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
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

        private SiteBuilderSettings createSiteBuilderSettings(IServiceProvider s)
        {
            var projectFinder = s.GetRequiredService<ProjectFinder>();

            return new SiteBuilderSettings()
            {
                InDir = projectFinder.PublishedProjectPath,
                BackupPath = projectFinder.BackupPath,
                OutDir = EdityServerConfiguration["OutputPath"],
                CompiledVirtualFolder = EdityServerConfiguration["CompiledVirtualFolder"],
                SiteName = EdityServerConfiguration["SiteName"]
            };
        }
    }
}
