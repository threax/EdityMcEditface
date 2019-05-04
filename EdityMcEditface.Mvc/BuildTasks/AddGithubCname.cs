using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Auth;
using EdityMcEditface.Mvc.BuildTasks;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    /// <summary>
    /// Publish a repo to GitHub. This assumes that the cloned publish destination is already
    /// pointing at github.
    /// </summary>
    public class AddGithubCname : IBuildTask
    {
        private String host;

        public AddGithubCname(BuildTaskDefinition taskDefinition)
        {
            if(!taskDefinition.Settings?.ContainsKey("host") == true)
            {
                throw new InvalidOperationException("You must include a 'host' setting to use the AddGithubCname task.");
            }
            host = taskDefinition.Settings["host"] as String;
        }

        public Task Execute(BuildEventArgs args)
        {
            using(var writer = new StreamWriter(args.SiteBuilder.OpenOutputWriteStream("CNAME")))
            {
                writer.Write(host);
            }

            return Task.FromResult(0);
        }
    }
}
