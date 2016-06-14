using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageDefinition
    {
        public Dictionary<String, String> Vars { get; set; }

        public Dictionary<String, String> LinkedContent { get; set; }
    }
}
