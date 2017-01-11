using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Swashbuckle.Swagger.Model;
using EdityMcEditface.Mvc.Config;
using EdityMcEditface.Mvc;

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
        }

        public IConfigurationRoot Configuration { get; }

        public EditySettings EditySettings { get; private set; }

        public ProjectConfiguration ProjectConfiguration { get; private set; } = new ProjectConfiguration();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEdity(EditySettings, ProjectConfiguration);

            if (env.IsEnvironment("Development"))
            {
                services.AddConventionalSwagger(info);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = "/edity/auth/login",
                LogoutPath = "/edity/auth/logout"
            });

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

            if (env.IsEnvironment("Development"))
            {
                app.UseConventionalSwagger(info);
            }

            app.UseEdity();
        }
    }
}
