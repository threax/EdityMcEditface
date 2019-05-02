using EdityMcEditface.Mvc.Config;
using EdityMcEditface.Mvc.Models.Phase;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
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
        private IPhaseDetector branchDetector;
        private ILogger<OneRepoPerUser> logger;

        public OneRepoPerUser(EditySettings editySettings, IPhaseDetector branchDetector, ILogger<OneRepoPerUser> logger)
        {
            this.projectFolder = editySettings.ProjectPath;
            this.EdityCorePath = editySettings.EdityCorePath;
            this.SitePath = editySettings.SitePath;
            this.MasterRepoPath = Path.Combine(projectFolder, "Master");
            this.branchDetector = branchDetector;
            this.logger = logger;
        }

        public String GetUserProjectPath(String user)
        {
            String repoPath;
            if (user == null)
            {
                user = "null-reserved-user";
            }
            repoPath = Path.Combine(projectFolder, "UserRepos", user);
            
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
                var publishRepoPath = Path.Combine(projectFolder, "Publish/Publish");

                if (!Directory.Exists(publishRepoPath))
                {
                    logger.LogInformation($"Creating publish directory at {publishRepoPath}.");
                    Directory.CreateDirectory(publishRepoPath);
                }
                if (!Repository.IsValid(publishRepoPath))
                {
                    logger.LogInformation($"Cloning publish directory from {MasterRepoPath} to {publishRepoPath}.");
                    Repository.Clone(MasterRepoPath, publishRepoPath);
                }

                return publishRepoPath;
            }
        }

        public String EdityCorePath { get; private set; }

        public String SitePath { get; private set; }

        public String MasterRepoPath { get; private set; }
    }
}
