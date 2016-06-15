using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageStackItem
    {
        public PageDefinition PageDefinition { get; set; }

        public String Content { get; set; }

        public String PageScriptPath { get; set; }
    }
}
