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
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.BuildTasks;
using System.Reflection;
using Threax.AspNetCore.Halcyon.Ext;
using EdityMcEditface.Mvc.Controllers;
using Newtonsoft.Json;
using Halcyon.Web.HAL.Json;
using Microsoft.AspNetCore.Mvc;
using Threax.AspNetCore.Halcyon.ClientGen;
using EdityMcEditface.Mvc.Models.Branch;

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
                var projectFolder = projectFinder.GetCurrentProjectPath(userInfo.UniqueUserName);

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

                //wwwroot for javascript, not editable
                var wwwRootFilePermissions = new DefaultFileFinderPermissions();
                wwwRootFilePermissions.WritePermission.Permit = false;
                wwwRootFilePermissions.TreatAsContentPermission.Permissions = new PathBlacklist(edityFolderList);
                var wwwRootFileFinder = new FileFinder("wwwroot", wwwRootFilePermissions, siteFileFinder);

                //Project location
                var contentFolderPermissions = new DefaultFileFinderPermissions();
                contentFolderPermissions.TreatAsContentPermission.Permissions = new PathBlacklist(edityFolderList);
                return new FileFinder(projectFolder, contentFolderPermissions, wwwRootFileFinder);
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
            var baseUrl = HalcyonConventionOptions.HostVariable;
            if(editySettings.BaseUrl != null)
            {
                baseUrl += "/" + editySettings.BaseUrl;
            }

            var halOptions = new HalcyonConventionOptions()
            {
                BaseUrl = baseUrl,
                HalDocEndpointInfo = new HalDocEndpointInfo(typeof(EndpointDocController)),
                MakeAllControllersHalcyon = false
            };

            services.AddConventionalHalcyon(halOptions);

            var halClientGenOptions = new HalClientGenOptions()
            {
                SourceAssemblies = new Assembly[] { typeof(EdityMvcExtensions).GetTypeInfo().Assembly }
            };

            if(editySettings.AdditionalMvcLibraries != null)
            {
                halClientGenOptions.SourceAssemblies = halClientGenOptions.SourceAssemblies.Concat(editySettings.AdditionalMvcLibraries);
            }

            services.AddHalClientGen(halClientGenOptions);

            if (editySettings.Events == null)
            {
                editySettings.Events = new EdityEvents();
            }

            services.TryAddScoped<IWebConfigProvider>(s =>
            {
                return new DefaultWebConfigProvider(projectConfiguration.DefaultPage);
            });
            services.TryAddScoped<ITargetFileInfoProvider>(s =>
            {
                return new DefaultTargetFileInfoProvider(projectConfiguration.DefaultPage);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<WorkQueue, WorkQueue>();

            services.TryAddScoped<IUserInfo, DefaultUserInfo>();

            services.TryAddScoped<IBranchDetector>(s =>
            {
                var settings = new JsonSerializerSettings();
                settings.SetToHalcyonDefault();
                var serializer = JsonSerializer.Create(settings);
                return new CookieBranchDetector("edityBranch", projectConfiguration.PrepublishBranchName, serializer, s.GetRequiredService<IHttpContextAccessor>());
            });

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
                        return new OneRepoPerUser(projectConfiguration, s.GetRequiredService<IBranchDetector>());
                    });
                    break;
            }

            services.AddTransient<Repository, Repository>(s =>
            {
                var userInfo = s.GetRequiredService<IUserInfo>();
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                var projectFolder = projectFinder.GetCurrentProjectPath(userInfo.UniqueUserName);
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
                        var webConfigProvider = s.GetRequiredService<IWebConfigProvider>();
                        var builder = new RoundRobinSiteBuilder(settings, compilerFactory, fileFinder, new WebConfigRoundRobinDeployer(webConfigProvider));

                        if (projectConfiguration.ProjectMode == "OneRepoPerUser")
                        {
                            builder.addPreBuildTask(new PullPublish(projectFinder.MasterRepoPath, projectFinder.PublishedProjectPath));
                        }

                        editySettings.Events.CustomizeSiteBuilder(new SiteBuilderEventArgs()
                        {
                            SiteBuilder = builder,
                            Services = s
                        });

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

                        editySettings.Events.CustomizeSiteBuilder(new SiteBuilderEventArgs()
                        {
                            SiteBuilder = builder,
                            Services = s
                        });

                        return builder;
                    });
                    break;
            }

            // Add framework services.
            var mvcBuilder = services.AddMvc(o =>
            {
                o.UseExceptionErrorFilters(editySettings.DetailedErrors);
                o.UseConventionalHalcyon(halOptions);
            })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.SetToHalcyonDefault();
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddEdityControllers(editySettings.AdditionalMvcLibraries);

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

        private static IMvcBuilder AddEdityControllers(this IMvcBuilder builder, IEnumerable<Assembly> additionalAssemblies)
        {
            builder.AddApplicationPart(typeof(EdityMvcExtensions).GetTypeInfo().Assembly);
            if(additionalAssemblies != null)
            {
                foreach(var assembly in additionalAssemblies)
                {
                    builder.AddApplicationPart(assembly);
                }
            }
            return builder;
        }
    }
}
