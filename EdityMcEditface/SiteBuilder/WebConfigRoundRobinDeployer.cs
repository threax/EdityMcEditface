using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface
{
    public class WebConfigRoundRobinDeployer : RoundRobinDeployer
    {
        public bool Deploy(string outputFolder)
        {
            var innerFolder = Path.GetFileName(outputFolder);
            var outputPath = Path.GetDirectoryName(outputFolder);

            var webConfigFile = Path.Combine(outputPath, "web.config");
            var webConfigOutput = String.Format(webConfigContents, innerFolder);

            using (var writer = new StreamWriter(File.Open(webConfigFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(webConfigOutput);
            }

            return true;
        }

        private const String webConfigContents =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <system.webServer>
    <defaultDocument>
      <files>
        <add value=""{0}/index.html""/>
      </files>
    </defaultDocument>
    <rewrite>
      <rules>
        <rule name=""RewriteToGuidDir"">
          <match url=""^(.*)$"" ignoreCase=""false"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsFile"" ignoreCase=""false"" negate=""true"" />
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsDirectory"" ignoreCase=""false"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{0}/{{R:1}}"" />
        </rule>
        <rule name=""RewriteHtmlToGuidDir"">
          <match url=""(.*)"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsFile"" negate=""true"" />
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsDirectory"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{{R:1}}.html"" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>";
    }
}
