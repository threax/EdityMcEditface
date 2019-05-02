using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class BuildTaskDefinition
    {
        public String Name { get; set; }

        public Dictionary<String, Object> Settings { get; set; }
    }
}
