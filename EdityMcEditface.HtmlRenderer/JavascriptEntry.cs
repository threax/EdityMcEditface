using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class JavascriptEntry
    {
        [JsonRequired]
        public String File { get; set; }

        public bool Async { get; set; } = false;
    }
}
