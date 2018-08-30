using EdityMcEditface.HtmlRenderer.SiteBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.BuildTasks
{
    public class SimpleWebConfigTask : IBuildTask
    {
        private ISiteBuilder siteBuilder;
        private String homePage;

        public SimpleWebConfigTask(ISiteBuilder siteBuilder, String homePage)
        {
            this.siteBuilder = siteBuilder;
            this.homePage = homePage;
        }

        public Task Execute(BuildEventArgs args)
        {
            args.Tracker.AddMessage("Creating web.config.");

            using (var writer = new StreamWriter(siteBuilder.OpenOutputWriteStream("web.config")))
            {
                writer.Write(CreateWebConfig());
            }
            return Task.FromResult(0);
        }

        public String CreateWebConfig()
        {
            var webConfig =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <system.webServer>";

            //Don't add index.html since iis will define that by default. This reduces the server configuration.
            if(!homePage.Equals("index", StringComparison.InvariantCultureIgnoreCase))
            {
                webConfig +=
$@"
    <defaultDocument>
      <files>
        <add value=""{homePage}.html""/>
      </files>
    </defaultDocument>";
            }

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
    </rewrite>
    <staticContent>
        <mimeMap fileExtension="".json"" mimeType=""application/json"" />
    </staticContent>
  </system.webServer>
</configuration>";

            return webConfig;
        }
    }
}
