using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class CompilerSettings
    {
        public String InDir { get; set; }

        public String BackupPath { get; set; }

        public String OutDir { get; set; }

        public String EdityDir { get; set; } = "edity";
    }
}
