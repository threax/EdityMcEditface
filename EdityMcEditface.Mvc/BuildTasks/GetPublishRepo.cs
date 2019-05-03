using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    /// <summary>
    /// This class will pull or clone a repo to use as the publish destination folder.
    /// </summary>
    public class GetPublishRepo : IBuildTask
    {
        private String url;

        public GetPublishRepo(BuildTaskDefinition buildTaskDefinition)
        {
            if (!buildTaskDefinition.Settings.ContainsKey("url"))
            {
                throw new InvalidOperationException("You must include a 'url' in the settings to use the GetPublishRepo build task.");
            }
            url = buildTaskDefinition.Settings["url"] as String;
        }

        public Task Execute(BuildEventArgs args)
        {
            var outDir = args.SiteBuilder.Settings.OutDir;
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            if (!Repository.IsValid(outDir))
            {
                args.Tracker.AddMessage($"Cloning publish repo from {url}.");
                Repository.Clone(url, outDir);
            }
            else
            {
                args.Tracker.AddMessage($"Pulling changes to publish repo from {url}.");
                using (Repository repo = new Repository(outDir))
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
