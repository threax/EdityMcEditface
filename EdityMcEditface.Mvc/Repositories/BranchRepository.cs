using EdityMcEditface.Mvc.Models.Branch;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Repositories
{
    public class BranchRepository
    {
        private Repository repository;

        public BranchRepository(Repository repository)
        {
            this.repository = repository;
        }

        public Models.Branch.BranchCollection List()
        {
            var branchEnum = this.repository.Branches.GetEnumerator();
            var branches = new List<BranchView>();
            while (branchEnum.MoveNext())
            {
                branches.Add(new BranchView() { CanonicalName = branchEnum.Current.CanonicalName, FriendlyName = branchEnum.Current.FriendlyName });
            }
            return new Models.Branch.BranchCollection(branches);
        }
    }
}
