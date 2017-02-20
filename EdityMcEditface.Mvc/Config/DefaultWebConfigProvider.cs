using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Config
{
    public class DefaultWebConfigProvider : IWebConfigProvider
    {
        public string WebConfigTemplate
        {
            get
            {
                return webConfigContents;
            }
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
            <add input=""{{REQUEST_URI}}"" pattern=""^/(Service-)"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{0}/{{R:1}}"" />
        </rule>
        <rule name=""RewriteHtmlToGuidDir"">
          <match url=""(.*)"" />
          <conditions logicalGrouping=""MatchAll"" >
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsFile"" negate=""true"" />
            <add input=""{{REQUEST_FILENAME}}"" matchType=""IsDirectory"" negate=""true"" />
            <add input=""{{REQUEST_URI}}"" pattern=""^/(Service-)"" negate=""true"" />
          </conditions>
          <action type=""Rewrite"" url=""{{R:1}}.html"" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>";
    }
}
