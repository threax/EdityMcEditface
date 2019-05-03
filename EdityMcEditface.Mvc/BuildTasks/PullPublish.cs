using EdityMcEditface.HtmlRenderer.SiteBuilder;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    /// <summary>
    /// This build task ensures a publish repo exists and pulls in any updates. 
    /// This is used as the source folder when building.
    /// </summary>
    /// <remarks>
    /// This task is in the mvc library since that is where libgit is included.
    /// </remarks>
    public class PullPublish : IBuildTask
    {
        private String masterRepoPath;
        private String publishRepoPath;

        public PullPublish(String masterRepoPath, String publishRepoPath)
        {
            this.masterRepoPath = masterRepoPath;
            this.publishRepoPath = publishRepoPath;
        }

        public Task Execute(BuildEventArgs args)
        {
            args.Tracker.AddMessage("Pulling changes to publish repository.");

            if (!Directory.Exists(publishRepoPath))
            {
                Directory.CreateDirectory(publishRepoPath);
            }
            if (!Repository.IsValid(publishRepoPath))
            {
                Repository.Clone(masterRepoPath, publishRepoPath);
            }
            else
            {
                using (Repository repo = new Repository(publishRepoPath))
                {
                    var result = Commands.Pull(repo, new Signature("PublishAgent", "PublishAgent@notaperson.com", DateTime.Now), new PullOptions());
                    switch (result.Status)
                    {
                        case MergeStatus.Conflicts:
                            throw new Exception("Pull from source resulted in conflicts, cannot publish. The publish repo will need to be fixed manually.");
                    }
                }
            }
            return Task.FromResult(0);
        }
    }
}
