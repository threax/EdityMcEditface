using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    public class CreateIISWebConfig : IBuildTask
    {
        public CreateIISWebConfig(BuildTaskDefinition definition)
        {
            
        }

        public Task Execute(BuildEventArgs args)
        {
            args.Tracker.AddMessage("Creating IIS web.config.");

            using (var writer = new StreamWriter(args.SiteBuilder.OpenOutputParentWriteStream("web.config")))
            {
                writer.Write(CreateWebConfig(args.SiteBuilder));
            }
            return Task.FromResult(0);
        }

        public String CreateWebConfig(ISiteBuilder siteBuilder)
        {
            var usingDeploymentFolder = false;
            var homePage = siteBuilder.Project.DefaultPage;
            if (siteBuilder.DeploymentSubFolder != null)
            {
                homePage = Path.Combine(siteBuilder.DeploymentSubFolder, homePage);
                usingDeploymentFolder = true;
            }

            var webConfig =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <system.webServer>";

            //Don't add index.html since iis will define that by default. This reduces the server configuration.
            if (!homePage.Equals("index", StringComparison.InvariantCultureIgnoreCase))
            {
                webConfig +=
$@"
    <defaultDocument>
      <files>
        <add value=""{homePage}.html""/>
      </files>
    </defaultDocument>";
            }

            if (usingDeploymentFolder)
            {
                webConfig +=
                    $@"
    <rewrite>
      <rules>
        <rule name=""RewriteToGuidDir"">
          <match url=""^(.*)$"" ignoreCase=""false"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsFile"" ignoreCase=""false"" negate=""true"" />
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsDirectory"" ignoreCase=""false"" negate=""true"" />
            <add input=""{{REQUEST_URI}}"" pattern=""^/(EmbdSvcs-)"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{siteBuilder.DeploymentSubFolder}/{{R:1}}"" />
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
    </rewrite>";
            }
            else
            {
                webConfig +=
    @"
    <rewrite>
      <rules>
        <rule name=""RewriteToHtml"">
          <match url=""(.*)"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{REQUEST_FILENAME}"" matchType=""IsFile"" negate=""true"" />
            <add input=""{REQUEST_FILENAME}"" matchType=""IsDirectory"" negate=""true"" />
            <add input=""{REQUEST_URI}"" pattern=""^/(EmbdSvcs-)"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{R:1}.html"" />
        </rule>
      </rules>
    </rewrite>";
            }
            webConfig +=
    @"
    <staticContent>
        <mimeMap fileExtension="".json"" mimeType=""application/json"" />
    </staticContent>
  </system.webServer>
</configuration>";

            return webConfig;
        }
    }
}
