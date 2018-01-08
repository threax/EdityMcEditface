using EdityMcEditface.Mvc.Models.Branch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private LibGit2Sharp.Repository repo;
        private string localRefRoot;
        private string remoteRefRoot;

        public BranchRepository(LibGit2Sharp.Repository repository, string localRefRoot = "refs/heads/", String remoteRefRoot = "refs/remotes/origin/")
        {
            this.repo = repository;
            this.localRefRoot = localRefRoot;
            this.remoteRefRoot = remoteRefRoot;
        }

        /// <summary>
        /// Get a list of branch names that can be activated.
        /// </summary>
        /// <returns></returns>
        public BranchCollection List()
        {
            //This will look at all local refs first, and then add remotes that have not already been identified.

            var branches = new List<LibGit2Sharp.Branch>();
            var ignoredRemoteBranches = new HashSet<LibGit2Sharp.Branch>();
            foreach(var branch in this.repo.Branches.Where((i => i.CanonicalName.StartsWith(localRefRoot))))
            {
                branches.Add(branch);
                if (branch.TrackedBranch != null)
                {
                    ignoredRemoteBranches.Add(branch.TrackedBranch);
                }
            }

            foreach(var branch in this.repo.Branches.Where((i => i.CanonicalName.StartsWith(remoteRefRoot) && !ignoredRemoteBranches.Contains(i))))
            {
                branches.Add(branch);
            }

            return new BranchCollection(branches.Select(b => new BranchView() { CanonicalName = b.CanonicalName, FriendlyName = Path.GetFileName(b.FriendlyName) }));
        }

        public void Add(String name)
        {
            var branch = LibGit2Sharp.RepositoryExtensions.CreateBranch(repo, name);
            LinkBranchToRemote(branch);
        }

        private void LinkBranchToRemote(LibGit2Sharp.Branch branch)
        {
            var remote = repo.Network.Remotes["origin"];
            if (remote != null)
            {
                repo.Branches.Update(branch, b => b.Remote = remote.Name, b => b.UpstreamBranch = branch.CanonicalName);
            }
        }

        public void Checkout(String name, LibGit2Sharp.Signature sig)
        {
            var localRef = localRefRoot + name;
            LibGit2Sharp.Branch branch = repo.Branches[localRef];

            var remoteRef = remoteRefRoot + name;
            var remoteBranch = repo.Branches[remoteRef];

            //Found a local branch, use it
            if (branch != null)
            {
                LibGit2Sharp.Commands.Checkout(repo, branch);
                if(remoteBranch != null && remoteBranch.Tip != repo.Head.Tip)
                {
                    repo.Merge(remoteBranch, sig, new LibGit2Sharp.MergeOptions());
                }
                return; //Was able to do a simple checkout to a local branch
            }

            //No local branch, use the remote branch and create a new local branch
            if(remoteBranch != null)
            {
                //Since we already know there is not a local branch, create it
                var localBranch = repo.Branches.Add(name, remoteBranch.Tip);
                LinkBranchToRemote(localBranch);
                LibGit2Sharp.Commands.Checkout(repo, localBranch);
                return; //Was able to find branch in remote repo. Checkout to it
            }

            throw new InvalidOperationException($"Cannot find branch {name} in current local or remote branches. Do you need to create the branch or pull in updates?");
        }
    }
}
