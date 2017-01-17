﻿using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Models.Page;
using LibGit2Sharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This controller compiles the static website.
    /// </summary>
    [Route("edity/[controller]/[action]")]
    [Authorize(Roles=Roles.Compile)]
    public class CompileController : Controller
    {
        private SiteBuilder builder;
        private WorkQueue workQueue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="workQueue"></param>
        public CompileController(SiteBuilder builder, WorkQueue workQueue)
        {
            this.builder = builder;
            this.workQueue = workQueue;
        }

        /// <summary>
        /// Get the current status of the compiler.
        /// </summary>
        /// <param name="projectFinder">The project finder.</param>
        /// <returns></returns>
        [HttpGet]
        public CompilerStatus Status([FromServices] ProjectFinder projectFinder)
        {
            var publishRepoPath = projectFinder.PublishedProjectPath;
            var masterRepoPath = projectFinder.MasterRepoPath;

            if (publishRepoPath != masterRepoPath)
            {
                if (!Directory.Exists(publishRepoPath))
                {
                    Directory.CreateDirectory(publishRepoPath);
                }
                if (!Repository.IsValid(publishRepoPath))
                {
                    Repository.Clone(masterRepoPath, publishRepoPath);
                }

                using (var repo = new Repository(publishRepoPath))
                {
                    repo.Fetch("origin");

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
        [HttpPost]
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