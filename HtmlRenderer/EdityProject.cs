using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class EdityProject
    {
        [JsonProperty]
        private Dictionary<String, LinkedContentEntry> contentLinks = new Dictionary<String, LinkedContentEntry>();

        [JsonIgnore]
        public IEnumerable<LinkedContentEntry> ContentLinks
        {
            get
            {
                return contentLinks.Values;
            }
        }

        public Dictionary<String, String> Vars { get; set; } = new Dictionary<string, string>();
    }
}
