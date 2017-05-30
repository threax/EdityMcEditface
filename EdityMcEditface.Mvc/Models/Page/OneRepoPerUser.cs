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
        private String projectFolder;
        private String liveBranchName;
        private IBranchDetector branchDetector;

        public OneRepoPerUser(ProjectConfiguration projectConfig, IBranchDetector branchDetector)
        {
            this.liveBranchName = projectConfig.LiveBranchName;
            this.projectFolder = projectConfig.ProjectPath;
            this.EdityCorePath = projectConfig.EdityCorePath;
            this.SitePath = projectConfig.SitePath;
            this.MasterRepoPath = Path.Combine(projectFolder, "Master");
            this.branchDetector = branchDetector;
        }

        public String GetUserProjectPath(String user)
        {
            String repoPath;
            String branch = null;
            if (IsLiveBranch)
            {
                branch = liveBranchName;
                repoPath = Path.Combine(projectFolder, liveBranchName);
            }
            else
            {
                if (user == null)
                {
                    user = "null-reserved-user";
                }
                repoPath = Path.Combine(projectFolder, "UserRepos", user);
            }
            
            if (!Directory.Exists(repoPath))
            {
                Directory.CreateDirectory(repoPath);
            }
            if (!Repository.IsValid(repoPath))
            {
                Repository.Clone(MasterRepoPath, repoPath, new CloneOptions()
                {
                    BranchName = branch
                });
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
                    var branch = query.Where(b => b.FriendlyName == this.liveBranchName).FirstOrDefault();
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

        public bool IsLiveBranch
        {
            get
            {
                return this.branchDetector.RequestedBranch == liveBranchName;
            }
        }
    }
}
