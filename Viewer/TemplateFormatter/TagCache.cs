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

        public bool tryGetTag(T tag, out String ret)
        {
            ret = null;
            bool result = false;
            if (!inlineTemplateCache.TryGetValue(tag, out ret))
            {
                String fileName = $"{tag}.html";
                if (templateDirectory.Contains(fileName))
                {
                    try
                    {
                        ret = File.ReadAllText(fileName);
                        result = true;
                    }
                    catch (Exception) { }
                }
                inlineTemplateCache.Add(tag, ret);
            }
            return result;
        }
    }
}
