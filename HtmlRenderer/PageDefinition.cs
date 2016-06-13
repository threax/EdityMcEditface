using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageDefinition
    {
        [JsonProperty]
        private Dictionary<String, String> vars = new Dictionary<string, string>();

        [JsonProperty]
        private Dictionary<String, String> linkedContent = new Dictionary<string, String>();
    }
}
