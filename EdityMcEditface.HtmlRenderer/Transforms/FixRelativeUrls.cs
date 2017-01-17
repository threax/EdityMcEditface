using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class FixRelativeUrls : ServerSideTransform
    {
        private String baseUrl;

        public FixRelativeUrls(String baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions)
        {
            replaceUrlInAttributes("href", document);
            replaceUrlInAttributes("src", document);
            replaceUrlInAttributes("data-hr-model-src", document);
            replaceUrlInAttributes("data-hr-config-urlroot", document);
        }

        private void replaceUrlInAttributes(String attr, HtmlDocument document)
        {
            foreach (var node in document.DocumentNode.Select($"[{attr}]"))
            {
                var path = node.GetAttributeValue(attr, "");
                bool singleSlash = path.Length > 0 && (path[0] == '\\' || path[0] == '/');
                bool doubleSlash = path.Length > 1 && ((path[0] == '\\' && path[1] == '\\') || (path[0] == '/' && path[1] == '/'));
                if (singleSlash && !doubleSlash)
                {
                    node.SetAttributeValue(attr, baseUrl + path);
                }
            }
        }
    }
}
