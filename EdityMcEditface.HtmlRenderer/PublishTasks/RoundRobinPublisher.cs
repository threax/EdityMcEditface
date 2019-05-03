using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.PublishTasks
{
    public class RoundRobinPublisher : IPublishTask
    {
        String outputFolder;

        public RoundRobinPublisher(String outputFolder)
        {
            this.outputFolder = outputFolder;
        }

        public Task Execute(BuildEventArgs args)
        {
            args.Tracker.AddMessage("Erasing old deployments.");

            var outputParent = Path.GetDirectoryName(outputFolder);

            //Delete old virtual directories
            foreach (var dir in Directory.EnumerateDirectories(outputParent))
            {
                if (dir != outputFolder)
                {
                    try
                    {
                        IOExtensions.MultiTryDirDelete(dir, true);
                    }
                    catch (Exception) { }
                }
            }

            return Task.FromResult(0);
        }
    }
}
