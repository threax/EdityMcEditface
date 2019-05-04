using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    /// <summary>
    /// Publish a repo to GitHub. This assumes that the cloned publish destination is already
    /// pointing at github.
    /// </summary>
    public class PublishToGitRepo : IBuildTask
    {
        private IGitCredentialsProvider gitCredentialsProvider = null;

        public PublishToGitRepo(BuildTaskDefinition taskDefinition)
        {
            
        }

        public PublishToGitRepo(BuildTaskDefinition taskDefinition, IGitCredentialsProvider gitCredentialsProvider)
        {
            this.gitCredentialsProvider = gitCredentialsProvider;
        }

        public Task Execute(BuildEventArgs args)
        {
            var outDir = args.SiteBuilder.Settings.OutDir;
            using (Repository repo = new Repository(outDir))
            {
                //Commit changes
                args.Tracker.AddMessage("Committing changes to publish repo.");
                if (!repo.Index.IsFullyMerged)
                {
                    throw new InvalidOperationException("Cannot commit while there are conflicts. Please resolve these first.");
                }

                var status = repo.RetrieveStatus();
                var signature = new Signature(args.UserInfo.Name, args.UserInfo.Email, DateTime.Now);

                if (status.IsDirty)
                {
                    foreach (var path in QueryChanges(status).Select(s => s.FilePath))
                    {
                        Commands.Stage(repo, path);
                    }
                    repo.Commit("Published from Edity McEditface.", signature, signature);
                }

                //Push
                args.Tracker.AddMessage("Pushing publish repo to destination.");
                var pushOptions = new PushOptions();
                if(gitCredentialsProvider != null)
                {
                    pushOptions.CredentialsProvider = gitCredentialsProvider.GetCredentials;
                }
                repo.Network.Push(repo.Head, pushOptions);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Helper function to query changes in the git repo.
        /// </summary>
        /// <param name="status">The status of the repository.</param>
        /// <returns></returns>
        private IEnumerable<StatusEntry> QueryChanges(RepositoryStatus status)
        {
            return status.Where(s => s.State != FileStatus.Ignored);
        }
    }
}
