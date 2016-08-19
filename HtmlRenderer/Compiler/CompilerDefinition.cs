using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public enum CompilerTypes
    {
        Html,
        Pdf,
        Json,
    }

    public class CompilerDefinition
    {
        public CompilerTypes Type { get; set; } = CompilerTypes.Html;

        public String Template { get; set; } = "default";

        public String Extension { get; set; } = null;

        public Dictionary<String, String> Settings { get; set; }
    }
}
