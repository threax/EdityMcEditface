using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Git;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public class SyncRepository : ISyncRepository
    {
        private ICommitRepository commitRepo;
        private Repository repo;

        public SyncRepository(Repository repo, ICommitRepository commitRepo)
        {
            this.repo = repo;
            this.commitRepo = commitRepo;
        }

        /// <summary>
        /// Get the files that need to be synced.
        /// </summary>
        /// <returns>A SyncInfo with the info.</returns>
        public async Task<SyncInfo> SyncInfo()
        {
            return await Task.Run<SyncInfo>(() =>
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

                try
                {
                    var result = Commands.Pull(repo, signature, new PullOptions());
                    switch (result.Status)
                    {
                        case MergeStatus.Conflicts:
                            throw new Exception("Pull from source resulted in conflicts, please resolve them manually.");
                    }
                }
                catch (LibGit2SharpException ex)
                {
                    throw new Exception(ex.Message);
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

                try
                {
                    repo.Network.Push(repo.Head, new PushOptions());
                }
                catch (LibGit2SharpException ex)
                {
                    throw new Exception(ex.Message);
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
