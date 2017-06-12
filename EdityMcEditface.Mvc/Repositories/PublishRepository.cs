using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Models.Page;
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
        private WorkQueue workQueue;
        private ProjectFinder projectFinder;

        public PublishRepository(SiteBuilder builder, WorkQueue workQueue, ProjectFinder projectFinder)
        {
            this.builder = builder;
            this.workQueue = workQueue;
            this.projectFinder = projectFinder;
        }

        /// <summary>
        /// Get the current status of the compiler.
        /// </summary>
        /// <returns></returns>
        public CompilerStatus Status()
        {
            var publishRepoPath = projectFinder.PublishedProjectPath;
            var masterRepoPath = projectFinder.MasterRepoPath;

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

                    return new CompilerStatus()
                    {
                        BehindBy = divergence.BehindBy.GetValueOrDefault(),
                        BehindHistory = behindCommits.Select(c => new History(c)).ToList()
                    };
                }
            }

            return new CompilerStatus()
            {
                BehindBy = -1,
                BehindHistory = null
            };
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        /// <returns>The time statistics when the compilation is complete.</returns>
        public async Task<CompilerResult> Compile()
        {
            Stopwatch sw = new Stopwatch();

            await workQueue.FireAsync(() =>
            {
                sw.Start();
                builder.BuildSite();
                sw.Stop();
            });

            return new CompilerResult()
            {
                ElapsedSeconds = sw.Elapsed.TotalSeconds
            };
        }
    }
}
