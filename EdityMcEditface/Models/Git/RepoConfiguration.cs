using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Git
{
    public class RepoConfiguration
    {
        private String projectDir;

        public RepoConfiguration(String projectDir)
        {
            this.projectDir = projectDir;
        }

        public Repository getUserRepository(String userName)
        {
            var repo = Path.Combine(projectDir, "UserRepos", userName);
            if (!Directory.Exists(repo))
            {
                Directory.CreateDirectory(repo);
            }
            return new Repository(repo);
        }

        public Repository getPublishedRepository()
        {
            var repo = Path.Combine(projectDir, "Published");
            if (!Directory.Exists(repo))
            {
                Directory.CreateDirectory(repo);
            }
            return new Repository(repo);
        }

        public Repository getMasterRepository()
        {
            var repo = Path.Combine(projectDir, "Master");
            if (!Directory.Exists(repo))
            {
                Directory.CreateDirectory(repo);
            }
            return new Repository(repo);
        }
    }
}
