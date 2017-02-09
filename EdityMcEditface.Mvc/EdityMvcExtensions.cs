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
using EdityMcEditface.HtmlRenderer.Filesystem;

namespace EdityMcEditface.Mvc
{
    public static class EdityMvcExtensions
    {
        /// <summary>
        /// Add a default file finder that has a content folder and a backup location. The
        /// edity folder in the content folder is not considered content.
        /// This will only do something if you have not previously registered a IFileFinder service.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultFileFinder(this IServiceCollection services)
        {
            services.TryAddTransient<IFileFinder>(s =>
            {
                var userInfo = s.GetRequiredService<IUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetUserProjectPath(userInfo.UniqueUserName);

                //Folder blacklist
                var edityFolderList = new PathList();
                edityFolderList.AddDirectory("edity");

                //Editor core location
                var backupPermissions = new DefaultFileFinderPermissions();
                backupPermissions.WritePermission.Permit = false;
                backupPermissions.TreatAsContentPermission.Permit = false;
                var editorCoreFinder = new FileFinder(projectFinder.EdityCorePath, backupPermissions);

                //Site specific files, not editable
                var projectBackupPermissions = new DefaultFileFinderPermissions();
                projectBackupPermissions.WritePermission.Permit = false;
                projectBackupPermissions.TreatAsContentPermission.Permissions = new PathBlacklist(edityFolderList);
                var siteFileFinder = new FileFinder(projectFinder.SitePath, projectBackupPermissions, editorCoreFinder);

                //Project location
                var contentFolderPermissions = new DefaultFileFinderPermissions();
                contentFolderPermissions.TreatAsContentPermission.Permissions = new PathBlacklist(edityFolderList);
                return new FileFinder(projectFolder, contentFolderPermissions, siteFileFinder);
            });

            return services;
        }

        /// <summary>
        /// Add the main edity services. If you call this and can use the default IFileFinder, you don't need to
        /// do anything else, if you want to customize the file finder add it before calling this function.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="editySettings">The edity settings.</param>
        /// <param name="projectConfiguration">The edity project configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddEdity(this IServiceCollection services, EditySettings editySettings, ProjectConfiguration projectConfiguration)
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
                        return new OneRepo(projectConfiguration.ProjectPath, projectConfiguration.EdityCorePath, projectConfiguration.SitePath);
                    });
                    break;
                case "OneRepoPerUser":
                    services.AddTransient<ProjectFinder, OneRepoPerUser>(s =>
                    {
                        return new OneRepoPerUser(projectConfiguration.ProjectPath, projectConfiguration.EdityCorePath, projectConfiguration.SitePath);
                    });
                    break;
            }

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

            services.AddDefaultFileFinder();

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

            return services;
        }

        /// <summary>
        /// Use the EdityMcEditface mvc implementation. You can remove UseStaticFiles and UseMvc from your
        /// own startup when calling this function.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseEdity(this IApplicationBuilder app)
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

            return app;
        }

        private static IMvcBuilder AddEdityControllers(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(typeof(EdityMvcExtensions).Assembly);
            return builder;
        }
    }
}
