using EdityMcEditface.Mvc.Models.Branch;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Repositories
{
    public class BranchRepository
    {
        private LibGit2Sharp.Repository repository;

        public BranchRepository(LibGit2Sharp.Repository repository)
        {
            this.repository = repository;
        }

        public BranchCollection List()
        {
            var branchEnum = this.repository.Branches.GetEnumerator();
            var branches = new List<BranchView>();
            while (branchEnum.MoveNext())
            {
                branches.Add(new BranchView() { CanonicalName = branchEnum.Current.CanonicalName, FriendlyName = branchEnum.Current.FriendlyName });
            }
            return new BranchCollection(branches);
        }

        public void Add(String name)
        {
            var remote = repository.Network.Remotes["origin"];
            var branch = LibGit2Sharp.RepositoryExtensions.CreateBranch(repository, name);
            repository.Branches.Update(branch, b => b.Remote = remote.Name, b => b.UpstreamBranch = branch.CanonicalName);
        }
    }
}
