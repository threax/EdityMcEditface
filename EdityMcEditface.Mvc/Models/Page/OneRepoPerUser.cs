using EdityMcEditface.Mvc.Models.Branch;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class OneRepoPerUser : ProjectFinder
    {
        String projectFolder;

        public OneRepoPerUser(String projectFolder, String edityCorePath, String sitePath)
        {
            this.projectFolder = projectFolder;
            this.EdityCorePath = edityCorePath;
            this.SitePath = sitePath;
            this.MasterRepoPath = Path.Combine(projectFolder, "Master");
        }

        public String GetUserProjectPath(String user)
        {
            if(user == null)
            {
                user = "null-reserved-user";
            }
            var repoPath = Path.Combine(projectFolder, "UserRepos", user);
            if (!Directory.Exists(repoPath))
            {
                Directory.CreateDirectory(repoPath);
            }
            if (!Repository.IsValid(repoPath))
            {
                Repository.Clone(MasterRepoPath, repoPath);
            }
            return repoPath;
        }

        public String PublishedProjectPath
        {
            get
            {
                return Path.Combine(projectFolder, "Publish");
            }
        }

        public String EdityCorePath { get; private set; }

        public String SitePath { get; private set; }

        public String MasterRepoPath { get; private set; }

        public String PublishedBranch
        {
            get
            {
                //See if the repo has a branch called live, if so use it
                using (var repo = new Repository(MasterRepoPath))
                {
                    var query = repo.Branches.Where(b => !b.IsRemote);
                    var branch = query.Where(b => b.FriendlyName == "live").FirstOrDefault();
                    if (branch != null)
                    {
                        return branch.FriendlyName;
                    }
                }

                return null;
            }
        }

        public Task<BranchViewCollection> GetBranches()
        {
            //See if the repo has a branch called live, if so use it
            using (var repo = new Repository(MasterRepoPath))
            {
                var query = repo.Branches.Where(b => !b.IsRemote).Select(i => new BranchView()
                {
                    Name = i.FriendlyName,
                    Current = i.IsCurrentRepositoryHead
                });
                return Task.FromResult(new BranchViewCollection(query.ToList()));
            }
        }
    }
}
