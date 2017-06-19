using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class ExpandRootedPaths : ServerSideTransform
    {
        private String baseUrl;

        public ExpandRootedPaths(String baseUrl)
        {
            this.baseUrl = baseUrl.EnsureStartingPathSlash().TrimEnd('/', '\\');
        }

        public void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions)
        {
            replaceUrlInAttributes("href", document);
            replaceUrlInAttributes("src", document);
            replaceUrlInAttributes("data-hr-model-src", document);
            replaceUrlInAttributes("data-hr-config-urlroot", document);
            replaceUrlInAttributes("data-hr-config-menu", document);
        }

        private void replaceUrlInAttributes(String attr, HtmlDocument document)
        {
            foreach (var node in document.DocumentNode.Select($"[{attr}]"))
            {
                var path = node.GetAttributeValue(attr, "");
                if (path.Length >= 2 && (path[0] == '~' && (path[1] == '\\' || path[1] == '/')))
                {
                    node.SetAttributeValue(attr, baseUrl + path.Substring(1));
                }
            }
        }
    }
}
