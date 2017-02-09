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
    }
}
