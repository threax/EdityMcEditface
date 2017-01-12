using EdityMcEditface.Mvc.Auth;
using EdityMcEditface.Mvc.Config;
using EdityMcEditface;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Models.Page;
using LibGit2Sharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdityMcEditface.HtmlRenderer.Compiler;

namespace EdityMcEditface.Mvc
{
    public static class EdityMvcExtensions
    {
        public static void AddEdity(this IServiceCollection services, EditySettings editySettings, ProjectConfiguration projectConfiguration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<WorkQueue, WorkQueue>();

            services.TryAddScoped<IUserInfo, DefaultUserInfo>();

            services.AddSingleton<EditySettings>(s => editySettings);

            switch (projectConfiguration.ProjectMode)
            {
                case "SingleRepo":
                default:
                    services.AddTransient<ProjectFinder, OneRepo>(s =>
                    {
                        return new OneRepo(projectConfiguration.ProjectPath, projectConfiguration.BackupFilePath);
                    });
                    break;
                case "OneRepoPerUser":
                    services.AddTransient<ProjectFinder, OneRepoPerUser>(s =>
                    {
                        return new OneRepoPerUser(projectConfiguration.ProjectPath, projectConfiguration.BackupFilePath);
                    });
                    break;
            }

            services.AddTransient<IFileFinder, FileFinder1>(s =>
            {
                var userInfo = s.GetRequiredService<IUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.UniqueUserName);
                return new FileFinder1(projectFolder, projectFinder.BackupPath);
            });

            services.AddTransient<Repository, Repository>(s =>
            {
                var userInfo = s.GetRequiredService<IUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.UniqueUserName);
                return new Repository(Repository.Discover(projectFolder));
            });

            services.AddTransient<Signature, Signature>(s =>
            {
                var userInfo = s.GetRequiredService<IUserInfo>();
                return new Signature(userInfo.PrettyUserName, userInfo.Email, DateTime.Now);
            });

            services.AddTransient<SiteBuilderSettings, SiteBuilderSettings>(s =>
            {
                var projectFinder = s.GetRequiredService<ProjectFinder>();

                return new SiteBuilderSettings()
                {
                    OutDir = projectConfiguration.OutputPath,
                    SiteName = projectConfiguration.SiteName
                };
            });

            services.TryAddScoped<IContentCompilerFactory, ContentCompilerFactory>();

            switch (projectConfiguration.Compiler)
            {
                case "RoundRobin":
                    services.AddTransient<SiteBuilder, RoundRobinSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var settings = s.GetRequiredService<SiteBuilderSettings>();
                        var compilerFactory = s.GetRequiredService<IContentCompilerFactory>();
                        var fileFinder = s.GetRequiredService<IFileFinder>();
                        var builder = new RoundRobinSiteBuilder(settings, compilerFactory, fileFinder, new WebConfigRoundRobinDeployer());

                        if (projectConfiguration.ProjectMode == "OneRepoPerUser")
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
                        var settings = s.GetRequiredService<SiteBuilderSettings>();
                        var compilerFactory = s.GetRequiredService<IContentCompilerFactory>();
                        var fileFinder = s.GetRequiredService<IFileFinder>();
                        var builder = new DirectOutputSiteBuilder(settings, compilerFactory, fileFinder);

                        if (projectConfiguration.ProjectMode == "OneRepoPerUser")
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
                o.UseExceptionErrorFilters(editySettings.DetailedErrors);
            })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            .AddEdityControllers();
        }

        public static void UseEdity(this IApplicationBuilder app)
        {
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

        private static IMvcBuilder AddEdityControllers(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(typeof(EdityMvcExtensions).Assembly);
            return builder;
        }
    }
}
