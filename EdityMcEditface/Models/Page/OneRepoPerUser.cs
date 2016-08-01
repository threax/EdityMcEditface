using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Page
{
    public class OneRepoPerUser : ProjectFinder
    {
        String projectFolder;

        public OneRepoPerUser(String projectFolder, String backupPath)
        {
            this.projectFolder = projectFolder;
            this.BackupPath = backupPath;
        }

        public String GetUserProjectPath(String user)
        {
            return Path.Combine(projectFolder, "UserRepos", user);
        }

        public String PublishedProjectPath
        {
            get
            {
                return Path.Combine(projectFolder, "Publish");
            }
        }

        public String BackupPath { get; private set; }
    }
}
