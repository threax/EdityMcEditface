using System;
using System.Collections.Generic;
using System.IO;

namespace CommonMarkTools.Renderer
{
    class HtmlTagMap
    {
        public delegate HtmlRenderer GetRenderer(HtmlElements htmlTag);

        private GetRenderer tagProvider;
        private Dictionary<HtmlElements, HtmlRenderer> inlineTemplateCache = new Dictionary<HtmlElements, HtmlRenderer>();

        public HtmlTagMap(GetRenderer tagProvider)
        {
            this.tagProvider = tagProvider;
        }

        public bool tryGetTag(HtmlElements htmlTag, out HtmlRenderer ret)
        {
            ret = null;
            if (!inlineTemplateCache.TryGetValue(htmlTag, out ret))
            {
                ret = tagProvider(htmlTag);
                inlineTemplateCache.Add(htmlTag, ret);
            }
            return ret != null;
        }
    }
}
