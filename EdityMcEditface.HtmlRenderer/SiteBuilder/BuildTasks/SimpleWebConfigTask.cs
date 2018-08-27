using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder.BuildTasks
{
    public class SimpleWebConfigTask : BuildTask
    {
        private SiteBuilder siteBuilder;
        private String homePage;

        public SimpleWebConfigTask(SiteBuilder siteBuilder, String homePage)
        {
            this.siteBuilder = siteBuilder;
            this.homePage = homePage;
        }

        public void execute()
        {
            String contents = webConfigContents + homePage + webConfigContentsEnd;
            using (var writer = new StreamWriter(siteBuilder.OpenOutputWriteStream("web.config")))
            {
                writer.Write(contents);
            }
        }

        private const String webConfigContents =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <system.webServer>
    <defaultDocument>
      <files>
        <add value=""";

        private const String webConfigContentsEnd = @"""/>
      </files>
    </defaultDocument>
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
    }
}
