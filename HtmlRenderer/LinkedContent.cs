using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class LinkedContent
    {
        [JsonProperty]
        private Dictionary<String, LinkedContentEntry> entries = new Dictionary<String, LinkedContentEntry>();

        public LinkedContent()
        {
            
        }

        /// <summary>
        /// Add everything from other that is not already in this collection
        /// </summary>
        /// <param name="other"></param>
        public void merge(LinkedContent other)
        {
            foreach(var entry in other.entries)
            {
                if (!entries.ContainsKey(entry.Key))
                {
                    entries.Add(entry.Key, entry.Value);
                }
            }
        }

        public String renderCss()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var entry in entries.Values)
            {
                foreach(var css in entry.Css)
                {
                    sb.AppendLine($@"<link rel=""stylesheet"" href=""{css}"" type=""text/css"" />");
                }
            }
            return sb.ToString();
        }

        public String renderJavascript()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in entries.Values)
            {
                foreach (var js in entry.Javascript)
                {
                    sb.AppendLine($@"<script type=""text/javascript"" src=""{js}""></script>");
                }
            }
            return sb.ToString();
        }
    }
}
