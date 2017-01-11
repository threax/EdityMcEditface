﻿using LibGit2Sharp;
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

        public String BackupPath { get; private set; }

        public String MasterRepoPath { get; private set; }
    }
}