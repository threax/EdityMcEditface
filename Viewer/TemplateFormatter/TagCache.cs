using System;
using System.Collections.Generic;
using System.IO;

namespace Viewer.TemplateFormatter
{
    class TagCache<T>
    {
        private String templateDirectory;
        private Dictionary<T, String> inlineTemplateCache = new Dictionary<T, string>();

        public TagCache(String templateDirectory)
        {
            this.templateDirectory = templateDirectory;
        }

        public String getTag(T tag)
        {
            String ret;
            if (!inlineTemplateCache.TryGetValue(tag, out ret))
            {
                String fileName = $"{tag}.html";
                if (templateDirectory.Contains(fileName))
                {
                    ret = File.ReadAllText(fileName);
                }
                else
                {
                    ret = null;
                }
                inlineTemplateCache.Add(tag, ret);
            }
            return ret;
        }
    }
}
