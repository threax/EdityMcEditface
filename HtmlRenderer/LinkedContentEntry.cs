using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class LinkedContentEntry
    {
        [JsonProperty]
        private List<String> css = new List<string>();
        [JsonProperty]
        private List<JavascriptEntry> js = new List<JavascriptEntry>();

        public LinkedContentEntry(bool hasMin = true)
        {
            this.HasMin = hasMin;
        }

        public void addCss(String file)
        {
            css.Add(file);
        }

        public void addJavascript(JavascriptEntry file)
        {
            js.Add(file);
        }

        public bool HasMin { get; set; }

        public List<String> Dependencies { get; set; } = new List<String>();

        [JsonIgnore]
        public IEnumerable<String> Css
        {
            get
            {
                return css;
            }
        }

        [JsonIgnore]
        public IEnumerable<JavascriptEntry> Javascript
        {
            get
            {
                return js;
            }
        }
    }
}
