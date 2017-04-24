using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EdityMcEditface.HtmlRenderer
{
    public class LinkedContentEntry
    {
        public LinkedContentEntry()
        {
            
        }

        public void addCss(String file)
        {
            Css.Add(file);
        }

        public void addJavascript(JavascriptEntry file)
        {
            Js.Add(file);
        }

        public List<String> Dependencies { get; set; } = new List<String>();

        public List<String> Css { get; set; } = new List<String>();

        public List<JavascriptEntry> Js { get; set; } = new List<JavascriptEntry>();
    }
}
