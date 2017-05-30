using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models.Branch;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class OneRepo : ProjectFinder
    {
        String projectFolder;

        public OneRepo(String projectFolder, String edityCorePath, String sitePath)
        {
            this.projectFolder = projectFolder;
            this.EdityCorePath = edityCorePath;
            this.SitePath = sitePath;
            this.MasterRepoPath = projectFolder;
        }

        public String GetCurrentProjectPath(String user)
        {
            return projectFolder;
        }

        public Task<BranchViewCollection> GetBranches()
        {
            return Task.FromResult(new BranchViewCollection(new BranchView[] { new BranchView() { Name = "master", Current = true } }));
        }

        public String PublishedProjectPath
        {
            get
            {
                return projectFolder;
            }
        }

        public String EdityCorePath { get; private set; }

        public String SitePath { get; private set; }

        public String MasterRepoPath { get; private set; }

        public String PublishedBranch
        {
            get
            {
                return null;
            }
        }

        public bool IsPrepublishBranch
        {
            get
            {
                return false;
            }
        }
    }
}
