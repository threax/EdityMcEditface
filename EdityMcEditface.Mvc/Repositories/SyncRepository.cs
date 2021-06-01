using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Git;
using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Threax.ProcessHelper;

namespace EdityMcEditface.Mvc.Repositories
{
    public class SyncRepository : ISyncRepository
    {
        private IGitCredentialsProvider gitCredentialsProvider;
        private readonly IProcessRunner processRunner;
        private ICommitRepository commitRepo;
        private Repository repo;

        public SyncRepository(Repository repo, ICommitRepository commitRepo, IGitCredentialsProvider gitCredentialsProvider, IProcessRunner processRunner)
        {
            this.repo = repo;
            this.commitRepo = commitRepo;
            this.gitCredentialsProvider = gitCredentialsProvider;
            this.processRunner = processRunner;
        }

        /// <summary>
        /// Get the files that need to be synced.
        /// </summary>
        /// <returns>A SyncInfo with the info.</returns>
        public async Task<SyncInfo> SyncInfo()
        {
            return await Task.Run<SyncInfo>(() =>
            {
                var startInfo = new ProcessStartInfo("git") { ArgumentList = { "fetch", "--all" }, WorkingDirectory = repo.Info.WorkingDirectory };
                var exit = processRunner.Run(startInfo);
                if (exit != 0)
                {
                    throw new InvalidOperationException($"Exit code {exit} running git fetch");
                }

                var head = repo.Head.Commits.First();
                //No tracked branch, cannot sync anything, return empty result
                if(repo.Head.TrackedBranch == null)
                {
                    return new SyncInfo()
                    {
                        HasUncomittedChanges = commitRepo.HasUncommittedChanges(),
                        AheadBy = 0,
                        BehindBy = 0,
                        AheadHistory = Enumerable.Empty<History>(),
                        BehindHistory = Enumerable.Empty<History>()
                    };
                }

                //Head has a tracked branch, can handle sync
                var tracked = repo.Head.TrackedBranch.Commits.First();
                var divergence = repo.ObjectDatabase.CalculateHistoryDivergence(head, tracked);

                var aheadCommits = repo.Commits.QueryBy(new CommitFilter()
                {
                    SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
                    IncludeReachableFrom = divergence.One,
                    ExcludeReachableFrom = divergence.CommonAncestor
                });

                var behindCommits = repo.Commits.QueryBy(new CommitFilter()
                {
                    SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
                    IncludeReachableFrom = divergence.Another,
                    ExcludeReachableFrom = divergence.CommonAncestor
                });

                return new SyncInfo()
                {
                    HasUncomittedChanges = commitRepo.HasUncommittedChanges(),
                    AheadBy = divergence.AheadBy.GetValueOrDefault(),
                    BehindBy = divergence.BehindBy.GetValueOrDefault(),
                    AheadHistory = aheadCommits.Select(c => new History(c)),
                    BehindHistory = behindCommits.Select(c => new History(c))
                };
            });
        }

        /// <summary>
        /// Pull in changes from the origin repo.
        /// </summary>
        /// <param name="signature">The signature to use, from services.</param>
        public async Task Pull(Signature signature)
        {
            await Task.Run(() =>
            {
                if (commitRepo.HasUncommittedChanges())
                {
                    throw new InvalidOperationException("Cannot pull with uncommitted changes. Please commit first and try again.");
                }

                var startInfo = new ProcessStartInfo("git") { ArgumentList = { "pull", "origin" }, WorkingDirectory = repo.Info.WorkingDirectory };
                var exit = processRunner.Run(startInfo);
                if (exit != 0)
                {
                    throw new InvalidOperationException($"Exit code {exit} running git pull");
                }
            });
        }

        /// <summary>
        /// Push changes to the origin repo.
        /// </summary>
        public async Task Push()
        {
            await Task.Run(() =>
            {
                if (commitRepo.HasUncommittedChanges())
                {
                    throw new InvalidOperationException("Cannot push with uncommitted changes. Please commit first and try again.");
                }

                var startInfo = new ProcessStartInfo("git") { ArgumentList = { "push", "origin", "--all" }, WorkingDirectory = repo.Info.WorkingDirectory };
                var exit = processRunner.Run(startInfo);
                if(exit != 0)
                {
                    throw new InvalidOperationException($"Exit code {exit} running git push");
                }
            });

            //Garbage collect using git gc, if it is not installed this will silently fail
            //This kind of works, but can hang waiting for input, so disabled for now.
            //try
            //{
            //    await Task.Run(() =>
            //    {
            //        var p = Process.Start(new ProcessStartInfo()
            //        {
            //            FileName = "git",
            //            Arguments = "gc",
            //            CreateNoWindow = true,
            //            WorkingDirectory = repo.Info.WorkingDirectory,
            //            UseShellExecute = true
            //        });
            //        p.WaitForExit(300000); //Wait for 5 minutes, may need adjustment
            //    });
            //}
            //catch (Exception)
            //{

            //}
        }
    }
}
