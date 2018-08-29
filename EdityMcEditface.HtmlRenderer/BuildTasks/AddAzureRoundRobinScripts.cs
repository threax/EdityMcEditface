using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    /// <summary>
    /// This class will add 3 files to your deployment that will use a round robin style to deploy
    /// your site instead of writing it directly to wwwroot. This works the same way as the round
    /// robin publisher in this app, but it will run on the azure instance
    /// </summary>
    public class AddAzureRoundRobinScripts : IBuildTask
    {
        private String defaultPage;
        private ISiteBuilder siteBuilder;

        public AddAzureRoundRobinScripts(ISiteBuilder siteBuilder, String defaultPage)
        {
            this.defaultPage = defaultPage;
            this.siteBuilder = siteBuilder;
        }

        public Task Execute()
        {
            using (var writer = new StreamWriter(siteBuilder.OpenOutputWriteStream(".deployment")))
            {
                writer.Write(dotDeployment);
            }

            using (var writer = new StreamWriter(siteBuilder.OpenOutputWriteStream("deploy.ps1")))
            {
                writer.Write(deployPs1);
            }

            using (var writer = new StreamWriter(siteBuilder.OpenOutputWriteStream("webconfig.txt")))
            {
                writer.Write(GetWebConfigTemplate());
            }

            return Task.FromResult(0);
        }

        private String dotDeployment =
@"[config]
COMMAND=powershell -NoProfile -NoLogo -ExecutionPolicy Unrestricted -Command ""& ""$pwd\deploy.ps1"" 2>&1 | echo""";

        private String deployPs1 =
@"function FindAndReplace {
param([string]$inputFile, [string]$outputFile, [string]$findString, [string]$replaceString)
    (Get-Content $inputFile) | foreach {$_.replace($findString,$replaceString)} | Set-Content $outputFile
}

$DeploymentId=New-Guid

# Read environment variables
$DeploymentTarget=$env:DEPLOYMENT_TARGET
$DeploymentSource=$env:DEPLOYMENT_SOURCE

New-Item -ItemType Directory -Path $DeploymentTarget\$DeploymentId
Copy-Item $DeploymentSource\* -Destination $DeploymentTarget\$DeploymentId -Recurse -Exclude @("".deployment"",""deploy.ps1"",""webconfig.txt"")

FindAndReplace ""webconfig.txt"" ""$DeploymentTarget\web.config"" ""EDITY_DEPLOYMENT_ID"" $DeploymentId;

Get-ChildItem $DeploymentTarget -Exclude $DeploymentId,web.config | Remove-Item -Recurse
";

        private String GetWebConfigTemplate()
        {
            return 
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <system.webServer>
    <defaultDocument>
      <files>
        <add value=""EDITY_DEPLOYMENT_ID/{defaultPage}.html""/>
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
          <action type=""Rewrite"" url=""EDITY_DEPLOYMENT_ID/{{R:1}}"" />
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
