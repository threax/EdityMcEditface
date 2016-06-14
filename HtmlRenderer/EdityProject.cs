using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class EdityProject
    {
        public Dictionary<String, LinkedContentEntry> ContentMap { get; set; } = new Dictionary<string, LinkedContentEntry>();

        public Dictionary<String, String> Vars { get; set; } = new Dictionary<string, string>();

        public List<String> LinkedContent { get; set; } = new List<string>();
    }
}
