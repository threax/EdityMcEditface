﻿using EdityMcEditface.BuildTasks;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc;
using EdityMcEditface.Mvc.Config;
using EdityMcEditface.ToolControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Threax.NJsonSchema;
using Threax.NJsonSchema.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using Threax.AspNetCore.BuiltInTools;
using Threax.Extensions.Configuration.SchemaBinder;

namespace EdityMcEditface
{
    public class Startup
    {
        private String runningFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private IWebHostEnvironment env;

        public Startup(IWebHostEnvironment env)
        {
            this.env = env;

            String siteRootPath = runningFolder;
            if (env.EnvironmentName == "Development")
            {
                //In development mode use the main code folder as the site root, not the publish directory.
                siteRootPath = Path.GetFullPath(Path.Combine(runningFolder, "../../../"));
                if (!Directory.Exists(Path.Combine(siteRootPath, "ClientBin")))
                {
                    throw new Exception("Cannot find site root folder containing a ClientBin folder");
                }
            }

            var defaultProjectPath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(siteRootPath)
                .AddInMemoryCollection(new Dictionary<String, String>
                {
                    { "EditySettings:ReadFromCurrentDirectory", "false" },
                    { "EditySettings:NoAuth", "false" },
                    { "EditySettings:NoAuthUser", "OnlyUser" },
                    { "EditySettings:DetailedErrors", (env.EnvironmentName == "Development").ToString() },
                    { "EditySettings:ProjectMode", "OneRepo" },
                    { "EditySettings:OutputPath", Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"..\\{Path.GetFileName(Directory.GetCurrentDirectory())}-EdityOutput")) },
                    { "EditySettings:ProjectPath", defaultProjectPath },
                    { "EditySettings:EdityCorePath", Path.Combine(siteRootPath, "ClientBin/EdityMcEditface") },
                    { "EditySettings:SitePath", Path.Combine(siteRootPath, "ClientBin/Site") }
                })
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = new SchemaConfigurationBinder(builder.Build());
        }

        public SchemaConfigurationBinder Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddScoped<IGitCredentialsProvider, WindowsGitCredentialsProvider>();
            services.AddEdity(o =>
            {
                Configuration.Bind("EditySettings", o);

                o.Events.CustomizeBuildTasks = buildTasks =>
                {
                    buildTasks.SetBuildTaskBuilder("PublishToGitRepo", s => new PublishToGitRepo(s, new WindowsGitCredentialsProvider()));
                };

                o.Events.CustomizeTools = tools =>
                {
                    tools
                    .AddTool("updateConfigSchema", new ToolCommand("Update the schema file for this application's configuration.", async a =>
                    {
                        var json = await Configuration.CreateSchema();
                        File.WriteAllText("appsettings.schema.json", json);
                    }))
                    .AddTool("updateEditySchema", new ToolCommand("Update the schema for edity.json in the manual.", async a =>
                    {
                        var settings = new JsonSchemaGeneratorSettings()
                        {
                            FlattenInheritanceHierarchy = true,
                            DefaultPropertyNameHandling = PropertyNameHandling.CamelCase,
                            DefaultEnumHandling = EnumHandling.String,

                        };
                        var generator = new JsonSchemaGenerator(settings);
                        var schema = await generator.GenerateAsync(typeof(EdityProject));

                        File.WriteAllText("../Manual/edityschema.json", schema.ToJson());
                    }))
                    .AddTool("clone", new ToolCommand("Clone a git repo.", a => RepoTools.Clone(a, o)));

                    if (o.ProjectMode == ProjectMode.OneRepoPerUser)
                    {
                        tools.AddTool("pushmaster", new ToolCommand("Push a one repo per user shared repo to an origin. Only works in OneRepoPerUser mode.", a => RepoTools.PushMaster(a, o)));
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            if (env.EnvironmentName == "Development")
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
