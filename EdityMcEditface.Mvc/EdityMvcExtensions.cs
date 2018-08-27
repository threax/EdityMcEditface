﻿using AutoMapper;
using EdityMcEditface.BuildTasks;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.HtmlRenderer.Filesystem;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Auth;
using EdityMcEditface.Mvc.Config;
using EdityMcEditface.Mvc.Controllers;
using EdityMcEditface.Mvc.Models.Phase;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Templates;
using EdityMcEditface.Mvc.Repositories;
using EdityMcEditface.Mvc.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Threax.AspNetCore.BuiltInTools;
using Threax.AspNetCore.FileRepository;
using Threax.AspNetCore.Halcyon.ClientGen;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.SharedHttpClient;
using System.IO;
using EdityMcEditface.HtmlRenderer.SiteBuilder.BuildTasks;

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
                var phaseDetector = s.GetRequiredService<IPhaseDetector>();
                var compileRequestDetector = s.GetRequiredService<ICompileRequestDetector>();

                String projectFolder;
                if (compileRequestDetector.IsCompileRequest)
                {
                    projectFolder = projectFinder.PublishedProjectPath;
                }
                else
                {
                    projectFolder = projectFinder.GetUserProjectPath(userInfo.UniqueUserName);
                }

                //Any folders that can write should use these permissions, they will prevent the writing of .draft files.
                var sharedWritePermissions = new DraftFilePermissions(s.GetRequiredService<IHttpContextAccessor>());

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
                contentFolderPermissions.WritePermission.Permissions = sharedWritePermissions;

                //Always use the git draft manager for the project content, this way you can actually create drafts.
                var draftManager = new GitDraftManager(new PathBlacklist(edityFolderList));
                IFileStreamManager streamManager = null;

                if (phaseDetector.Phase == Phases.Draft || compileRequestDetector.IsCompileRequest)
                {
                    //If the request is in draft mode, change the content to only files with draft files and change the file stream
                    //manager to read published versions out of git
                    var oldPermissions = contentFolderPermissions.TreatAsContentPermission.Permissions;
                    contentFolderPermissions.TreatAsContentPermission.Permissions = new MustHaveGitDraftFile(draftManager, oldPermissions);
                    streamManager = new GitDraftFileStreamManager(draftManager);
                }

                return new FileFinder(projectFolder, contentFolderPermissions, wwwRootFileFinder, streamManager, draftManager);
            });

            return services;
        }

        /// <summary>
        /// Setup mappings, this is separate so it can be called by a unit test without
        /// spinning up the whole system. No need to call manually unless needed in a unit test.
        /// </summary>
        /// <returns></returns>
        private static MapperConfiguration SetupMappings()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Template, TemplateView>();
                cfg.CreateMap<BuildProgress, CompileProgress>();
            });

            return mapperConfig;
        }

        /// <summary>
        /// Add the main edity services. If you call this and can use the default IFileFinder, you don't need to
        /// do anything else, if you want to customize the file finder add it before calling this function.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="editySettings">The edity settings.</param>
        /// <param name="projectConfiguration">The edity project configuration.</param>
        /// <param name="setupServiceOptions">Callback to configure additional options for setting up services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddEdity(this IServiceCollection services, EditySettings editySettings, ProjectConfiguration projectConfiguration, Action<EdityServiceOptions> setupServiceOptions = null)
        {
            services.AddThreaxSharedHttpClient();

            var serviceOptions = new EdityServiceOptions();
            setupServiceOptions?.Invoke(serviceOptions);

            //Setup the mapper
            var mapperConfig = SetupMappings();
            services.AddScoped<IMapper>(i => mapperConfig.CreateMapper());

            //Setup repos
            services.TryAddScoped<ICommitRepository, CommitRepository>();
            services.TryAddScoped<ISyncRepository, SyncRepository>();
            services.TryAddScoped<IPathBaseInjector, PathBaseInjector>();
            services.TryAddScoped<IDraftRepository, DraftRepository>();
            services.TryAddScoped<IPublishRepository, PublishRepository>();
            services.TryAddScoped<IHistoryRepository, HistoryRepository>();
            services.TryAddScoped<IMergeRepository, MergeRepository>();
            services.TryAddScoped<IPageRepository, PageRepository>();
            services.TryAddScoped<ITemplateRepository, TemplateRepository>();
            services.TryAddScoped<IAssetRepository, AssetRepository>();
            services.TryAddScoped<IBranchRepository, BranchRepository>();
            services.TryAddSingleton<IOverrideValuesProvider>(s => new DefaultOverrideValuesProvider(projectConfiguration.OverrideVars));

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

            services.TryAddSingleton<WorkQueue, WorkQueue>();
            services.TryAddSingleton<ICompileService, CompileService>();

            services.TryAddScoped<IUserInfo, DefaultUserInfo>();

            services.TryAddScoped<IPhaseDetector>(s =>
            {
                var settings = new JsonSerializerSettings();
                settings.SetToHalcyonDefault();
                var serializer = JsonSerializer.Create(settings);
                return new CookiePhaseDetector("edityBranch", serializer, s.GetRequiredService<IHttpContextAccessor>());
            });

            services.AddSingleton<EditySettings>(s => editySettings);

            switch (projectConfiguration.ProjectMode)
            {
                case ProjectMode.OneRepo:
                default:
                    services.AddTransient<ProjectFinder, OneRepo>(s =>
                    {
                        return new OneRepo(projectConfiguration.ProjectPath, projectConfiguration.EdityCorePath, projectConfiguration.SitePath);
                    });
                    break;
                case ProjectMode.OneRepoPerUser:
                    services.AddTransient<ProjectFinder, OneRepoPerUser>(s =>
                    {
                        return new OneRepoPerUser(projectConfiguration, s.GetRequiredService<IPhaseDetector>());
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
                return new SiteBuilderSettings()
                {
                    OutDir = projectConfiguration.OutputPath,
                    SiteName = projectConfiguration.SiteName
                };
            });

            services.TryAddScoped<IContentCompilerFactory, ContentCompilerFactory>();

            services.AddDefaultFileFinder();

            services.TryAddTransient<PullPublish>(s =>
            {
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                return new PullPublish(projectFinder.MasterRepoPath, projectFinder.PublishedProjectPath);
            });

            switch (projectConfiguration.Compiler)
            {
                case Compilers.RoundRobin:
                    services.AddTransient<SiteBuilder, RoundRobinSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var settings = s.GetRequiredService<SiteBuilderSettings>();
                        var compilerFactory = s.GetRequiredService<IContentCompilerFactory>();
                        var fileFinder = s.GetRequiredService<IFileFinder>();
                        var webConfigProvider = s.GetRequiredService<IWebConfigProvider>();
                        var builder = new RoundRobinSiteBuilder(settings, compilerFactory, fileFinder, new WebConfigRoundRobinDeployer(webConfigProvider));

                        if (projectConfiguration.ProjectMode == ProjectMode.OneRepoPerUser)
                        {
                            builder.addPreBuildTask(s.GetRequiredService<PullPublish>());
                        }

                        editySettings.Events.CustomizeSiteBuilder(new SiteBuilderEventArgs()
                        {
                            SiteBuilder = builder,
                            Services = s
                        });

                        return builder;
                    });
                    break;
                case Compilers.RestEndpoint:
                    services.AddTransient<SiteBuilder, DirectOutputSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var settings = s.GetRequiredService<SiteBuilderSettings>();
                        settings.OutDir = Path.GetFullPath(Path.Combine(settings.OutDir, "azurezip")); //Change site to output to azurezip folder, that folder will be zipped
                        var compilerFactory = s.GetRequiredService<IContentCompilerFactory>();
                        var fileFinder = s.GetRequiredService<IFileFinder>();
                        var builder = new DirectOutputSiteBuilder(settings, compilerFactory, fileFinder); //Don't need a special site builder

                        if (projectConfiguration.ProjectMode == ProjectMode.OneRepoPerUser)
                        {
                            builder.addPreBuildTask(s.GetRequiredService<PullPublish>());
                        }

                        editySettings.Events.CustomizeSiteBuilder(new SiteBuilderEventArgs()
                        {
                            SiteBuilder = builder,
                            Services = s
                        });

                        //Add the RestPublisher last so any custom steps add their files correctly
                        builder.addPostBuildTask(new RestPublisher(projectConfiguration.RemotePublish, s.GetRequiredService<ISharedHttpClient>(), settings.OutDir));

                        return builder;
                    });
                    break;
                case Compilers.Direct:
                default:
                    services.AddTransient<SiteBuilder, DirectOutputSiteBuilder>(s =>
                    {
                        var projectFinder = s.GetRequiredService<ProjectFinder>();
                        var settings = s.GetRequiredService<SiteBuilderSettings>();
                        var compilerFactory = s.GetRequiredService<IContentCompilerFactory>();
                        var fileFinder = s.GetRequiredService<IFileFinder>();
                        var builder = new DirectOutputSiteBuilder(settings, compilerFactory, fileFinder);

                        if (projectConfiguration.ProjectMode == ProjectMode.OneRepoPerUser)
                        {
                            builder.addPreBuildTask(s.GetRequiredService<PullPublish>());
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

            services.AddExceptionErrorFilters(new ExceptionFilterOptions()
            {
                DetailedErrors = editySettings.DetailedErrors
            });

            // Add framework services.
            var mvcBuilder = services.AddMvc(o =>
            {
                o.UseExceptionErrorFilters();
                o.UseConventionalHalcyon(halOptions);
            })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.SetToHalcyonDefault();
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddEdityControllers(editySettings.AdditionalMvcLibraries);
            serviceOptions.CustomizeMvcBuilder?.Invoke(mvcBuilder);

            services.AddScoped<ICompileRequestDetector, CompileRequestDetector>();

            services.AddSingleton<IFileVerifier>(s =>
            {
                var verifier = new FileVerifier()
                    .AddHtml()
                    .AddBitmap()
                    .AddJpeg()
                    .AddPng()
                    .AddSvgXml()
                    .AddGif()
                    .AddPdf()
                    .AddDocx()
                    .AddDoc()
                    .AddPptx()
                    .AddPpt()
                    .AddXlsx()
                    .AddXls()
                    .AddJson();

                serviceOptions.CustomizeFileVerifier?.Invoke(verifier);

                return verifier;
            });

            services.AddScoped<IToolRunner>(s =>
            {
                var tools = new ToolRunner()
                    .UseClientGenTools();

                serviceOptions.CustomizeTools?.Invoke(tools);

                return tools;
            });

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
