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
        String homePage;

        public RoundRobinPublisher(String outputFolder, String homePage)
        {
            this.outputFolder = outputFolder;
            this.homePage = homePage;
        }

        public Task Execute()
        {
            var currentDeploymentFolder = Path.GetFileName(outputFolder);
            var outputParent = Path.GetDirectoryName(outputFolder);

            var webConfigFile = Path.Combine(outputParent, "web.config");
            var webConfigOutput = CreateWebConfig(currentDeploymentFolder);

            using (var writer = new StreamWriter(File.Open(webConfigFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(webConfigOutput);
            }

            //Delete old virtual directories
            foreach (var dir in Directory.EnumerateDirectories(outputParent))
            {
                if (dir != outputFolder)
                {
                    try
                    {
                        IOExtensions.MultiTryDirDelete(dir);
                    }
                    catch (Exception) { }
                }
            }

            return Task.FromResult(0);
        }

        private String CreateWebConfig(String currentDeploymentFolder)
        {
            return
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <system.webServer>
    <defaultDocument>
      <files>
        <add value=""{currentDeploymentFolder}/{homePage}.html""/>
      </files>
    </defaultDocument>
    <rewrite>
      <rules>
        <rule name=""RewriteToGuidDir"">
          <match url=""^(.*)$"" ignoreCase=""false"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsFile"" ignoreCase=""false"" negate=""true"" />
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsDirectory"" ignoreCase=""false"" negate=""true"" />
            <add input=""{{REQUEST_URI}}"" pattern=""^/(EmbdSvcs-)"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{currentDeploymentFolder}/{{R:1}}"" />
        </rule>
        <rule name=""RewriteHtmlToGuidDir"">
          <match url=""(.*)"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsFile"" negate=""true"" />
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsDirectory"" negate=""true"" />
            <add input=""{{REQUEST_URI}}"" pattern=""^/(EmbdSvcs-)"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{{R:1}}.html"" />
        </rule>
      </rules>
    </rewrite>
    <staticContent>
        <mimeMap fileExtension="".json"" mimeType=""application/json"" />
    </staticContent>
  </system.webServer>
</configuration>";
        }
    }
}
