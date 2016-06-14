using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageDefinition
    {
        public Dictionary<String, String> Vars { get; set; } = new Dictionary<String, String>();

        public List<String> LinkedContent { get; set; } = new List<string>();
    }
}
