using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Page
{
    public class ProjectFinder
    {
        String projectFolder;

        public ProjectFinder(String projectFolder, String backupPath)
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
                return Path.Combine(projectFolder, "Published");
            }
        }

        public String BackupPath { get; private set; }
    }
}
