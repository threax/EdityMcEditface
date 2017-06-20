using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Services;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public class PublishRepository : IPublishRepository
    {
        private SiteBuilder builder;
        private ICompileService compileService;
        private ProjectFinder projectFinder;

        public PublishRepository(SiteBuilder builder, ICompileService compileService, ProjectFinder projectFinder)
        {
            this.builder = builder;
            this.compileService = compileService;
            this.projectFinder = projectFinder;
        }

        /// <summary>
        /// Get the current status of the compiler.
        /// </summary>
        /// <returns></returns>
        public async Task<PublishEntryPoint> GetPublishInfo(ISyncRepository syncRepo)
        {
            var publishRepoPath = projectFinder.PublishedProjectPath;
            var masterRepoPath = projectFinder.MasterRepoPath;

            var compilerStatus = new PublishEntryPoint()
            {
                BehindBy = -1,
                BehindHistory = null
            };

            if (publishRepoPath != masterRepoPath)
            {
                using (var repo = new Repository(publishRepoPath))
                {
                    string logMessage = "";
                    foreach (Remote remote in repo.Network.Remotes)
                    {
                        var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                        Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
                    }

                    var head = repo.Head.Commits.First();
                    var tracked = repo.Head.TrackedBranch.Commits.First();
                    var divergence = repo.ObjectDatabase.CalculateHistoryDivergence(head, tracked);

                    var behindCommits = repo.Commits.QueryBy(new CommitFilter()
                    {
                        SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
                        IncludeReachableFrom = divergence.Another,
                        ExcludeReachableFrom = divergence.CommonAncestor
                    });

                    compilerStatus.BehindBy = divergence.BehindBy.GetValueOrDefault();
                    compilerStatus.BehindHistory = behindCommits.Select(c => new History(c)).ToList();
                    var syncInfo = await syncRepo.SyncInfo();
                    compilerStatus.HasUncommittedChanges = syncInfo.HasUncomittedChanges;
                    compilerStatus.HasUnsyncedChanges = syncInfo.NeedsPull || syncInfo.NeedsPush;
                }
            }

            return compilerStatus;
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        /// <returns>The time statistics when the compilation is complete.</returns>
        public void Compile()
        {
            this.compileService.Compile(builder);
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        public CompileProgress Progress()
        {
            return this.compileService.Progress();
        }
    }
}
