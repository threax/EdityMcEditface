using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Models.Compiler;
using EdityMcEditface.Models.Git;
using EdityMcEditface.Models.Page;
using LibGit2Sharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]")]
    [Authorize(Roles=Roles.Compile)]
    public class CompileController : Controller
    {
        private SiteBuilder builder;
        private WorkQueue workQueue;

        public CompileController(SiteBuilder builder, WorkQueue workQueue)
        {
            this.builder = builder;
            this.workQueue = workQueue;
        }

        [HttpGet("Status")]
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
                        BehindHistory = behindCommits.Select(c => new History(c))
                    };
                }
            }

            return new CompilerStatus()
            {
                BehindBy = -1,
                BehindHistory = null
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<CompilerResult> Index()
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
