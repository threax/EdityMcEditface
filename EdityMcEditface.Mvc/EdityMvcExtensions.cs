using AutoMapper;
using EdityMcEditface.BuildTasks;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.HtmlRenderer.Filesystem;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Auth;
using EdityMcEditface.Mvc.Config;
using EdityMcEditface.Mvc.Controllers;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Phase;
using EdityMcEditface.Mvc.Models.Templates;
using EdityMcEditface.Mvc.Repositories;
using EdityMcEditface.Mvc.Services;
using EdityMcEditface.PublishTasks;
using LibGit2Sharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Threax.AspNetCore.BuiltInTools;
using Threax.AspNetCore.FileRepository;
using Threax.AspNetCore.Halcyon.ClientGen;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.SharedHttpClient;

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
            services.TryAddScoped<IFileFinder>(s =>
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

                return new FileFinder(projectFolder, contentFolderPermissions, siteFileFinder, streamManager, draftManager);
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
        /// <param name="setupEditySettings">Callback to configure the edity settings.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddEdity(this IServiceCollection services, Action<EditySettings> setupEditySettings)
        {
            services.AddThreaxSharedHttpClient();

            var editySettings = new EditySettings();
            setupEditySettings.Invoke(editySettings);

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
            services.TryAddSingleton<IOverrideValuesProvider>(s => new DefaultOverrideValuesProvider(editySettings.OverrideVars));
            services.TryAddScoped<IGitCredentialsProvider, DefaultGitCredentialsProvider>();

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

            services.TryAddScoped<ITargetFileInfoProvider>(s =>
            {
                var fileFinder = s.GetRequiredService<IFileFinder>();
                return new DefaultTargetFileInfoProvider(fileFinder.Project.DefaultPage);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.TryAddSingleton<WorkQueue, WorkQueue>();
            services.TryAddSingleton<ICompileService>(s => new CompileService(s.GetRequiredService<WorkQueue>(), mapperConfig.CreateMapper()));

            services.TryAddScoped<IUserInfo, DefaultUserInfo>();

            services.TryAddScoped<IPhaseDetector>(s =>
            {
                var settings = new JsonSerializerSettings();
                settings.SetToHalcyonDefault();
                var serializer = JsonSerializer.Create(settings);
                return new CookiePhaseDetector("edityBranch", serializer, s.GetRequiredService<IHttpContextAccessor>());
            });

            services.AddSingleton<EditySettings>(s => editySettings);

            switch (editySettings.ProjectMode)
            {
                case ProjectMode.OneRepo:
                default:
                    services.AddTransient<ProjectFinder, OneRepo>(s =>
                    {
                        return new OneRepo(editySettings.ProjectPath, editySettings.EdityCorePath, editySettings.SitePath);
                    });
                    break;
                case ProjectMode.OneRepoPerUser:
                    services.AddTransient<ProjectFinder, OneRepoPerUser>(s =>
                    {
                        return new OneRepoPerUser(editySettings, s.GetRequiredService<IPhaseDetector>(), s.GetRequiredService<ILogger<OneRepoPerUser>>());
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
                    OutDir = editySettings.OutputPath
                };
            });

            services.TryAddScoped<IContentCompilerFactory, ContentCompilerFactory>();

            services.AddDefaultFileFinder();

            services.AddSingleton<BuildTaskManager>(s =>
            {
                var buildTaskManager = new BuildTaskManager();
                buildTaskManager.SetBuildTaskType("PublishMenu", typeof(PublishMenu));
                buildTaskManager.SetBuildTaskType("CreateIISWebConfig", typeof(CreateIISWebConfig));
                buildTaskManager.SetBuildTaskType("GetPublishRepo", typeof(GetPublishRepo));
                buildTaskManager.SetBuildTaskType("PublishToGitRepo", typeof(PublishToGitRepo));
                buildTaskManager.SetBuildTaskType("AddGithubCname", typeof(AddGithubCname));
                editySettings.Events.CustomizeBuildTasks?.Invoke(buildTaskManager);
                return buildTaskManager;
            });

            services.TryAddTransient<PullPublish>(s =>
            {
                var projectFinder = s.GetRequiredService<ProjectFinder>();
                return new PullPublish(projectFinder.MasterRepoPath, projectFinder.PublishedProjectPath);
            });

            services.AddTransient<ISiteBuilder>(s =>
            {
                var settings = s.GetRequiredService<SiteBuilderSettings>();
                var compilerFactory = s.GetRequiredService<IContentCompilerFactory>();
                var fileFinder = s.GetRequiredService<IFileFinder>();
                String deploymentFolder = null;
                if(editySettings.Publisher == Publishers.RoundRobin)
                {
                    deploymentFolder = Guid.NewGuid().ToString();
                }
                var builder = new SiteBuilder(settings, compilerFactory, fileFinder, deploymentFolder);
                var buildTaskManager = s.GetRequiredService<BuildTaskManager>();

                //Customize publisher settings depending on compiler setting
                switch (editySettings.Publisher)
                {
                    case Publishers.RoundRobin:
                        var outputBaseFolder = settings.OutDir;
                        settings.OutDir = Path.GetFullPath(Path.Combine(settings.OutDir, deploymentFolder));
                        builder.AddPublishTask(new RoundRobinPublisher(settings.OutDir));
                        break;
                }

                if (editySettings.ProjectMode == ProjectMode.OneRepoPerUser)
                {
                    builder.AddPreBuildTask(s.GetRequiredService<PullPublish>());
                }

                foreach(var preBuild in fileFinder.Project.PreBuildTasks)
                {
                    builder.AddPreBuildTask(buildTaskManager.CreateBuildTask(preBuild));
                }

                foreach (var postBuild in fileFinder.Project.PostBuildTasks)
                {
                    builder.AddPostBuildTask(buildTaskManager.CreateBuildTask(postBuild));
                }

                foreach (var publish in fileFinder.Project.PublishTasks)
                {
                    builder.AddPublishTask(buildTaskManager.CreateBuildTask(publish));
                }

                foreach (var postPublish in fileFinder.Project.PostPublishTasks)
                {
                    builder.AddPostPublishTask(buildTaskManager.CreateBuildTask(postPublish));
                }

                editySettings.Events.CustomizeSiteBuilder(new SiteBuilderEventArgs()
                {
                    SiteBuilder = builder,
                    Services = s
                });

                return builder;
            });

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
            .
#if NETCOREAPP3_0
            AddNewtonsoftJson
#elif NETSTANDARD2_0
            AddJsonOptions
#endif
            (o =>
            {
                o.SerializerSettings.SetToHalcyonDefault();
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddEdityControllers(editySettings.AdditionalMvcLibraries);
            editySettings.Events.CustomizeMvcBuilder?.Invoke(mvcBuilder);

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

                editySettings.Events.CustomizeFileVerifier?.Invoke(verifier);

                return verifier;
            });

            services.AddScoped<IToolRunner>(s =>
            {
                var tools = new ToolRunner()
                    .UseClientGenTools();

                editySettings.Events.CustomizeTools?.Invoke(tools);

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

#if NETCOREAPP3_0
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{*file}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });

#elif NETSTANDARD2_0
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{*file}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });
#endif


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
