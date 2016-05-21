using System;
using System.Collections.Generic;
using System.IO;

namespace Viewer.TemplateFormatter
{
    class HtmlTagMap
    {
        public delegate HtmlRenderer GetRenderer(String htmlTag);

        private GetRenderer tagProvider;
        private Dictionary<String, HtmlRenderer> inlineTemplateCache = new Dictionary<String, HtmlRenderer>();

        public HtmlTagMap(GetRenderer tagProvider)
        {
            this.tagProvider = tagProvider;
        }

        public bool tryGetTag(String htmlTag, out HtmlRenderer ret)
        {
            ret = null;
            bool result = false;
            if (!inlineTemplateCache.TryGetValue(htmlTag, out ret))
            {
                inlineTemplateCache.Add(htmlTag, tagProvider(htmlTag));
            }
            return result;
        }
    }
}
