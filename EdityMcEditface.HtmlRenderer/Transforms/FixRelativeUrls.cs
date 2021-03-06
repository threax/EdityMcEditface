﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class FixRelativeUrls : ServerSideTransform
    {
        private const String TestHost = "http://www.test.com";
        private static readonly Uri TestHostUri = new Uri(TestHost);

        private String baseUrl;
        private String fullBaseUrl; //Will be filesystem path, but only for comparison

        public FixRelativeUrls(String baseUrl)
        {
            this.baseUrl = baseUrl;
            this.fullBaseUrl = Path.GetFullPath(baseUrl);
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
                try
                {
                    var path = node.GetAttributeValue(attr, "");
                    bool singleSlash = path.Length > 0 && (path[0] == '\\' || path[0] == '/');
                    if (singleSlash)
                    {
                        bool doubleSlash = path.Length > 1 && ((path[0] == '\\' && path[1] == '\\') || (path[0] == '/' && path[1] == '/'));
                        if (!doubleSlash)
                        {
                            var uri = new Uri(TestHostUri, path);
                            var fullPath = Path.GetFullPath(uri.AbsolutePath);
                            if (!fullPath.StartsWith(fullBaseUrl, StringComparison.OrdinalIgnoreCase))
                            {
                                node.SetAttributeValue(attr, baseUrl + path);
                            }
                        }
                    }
                }
                catch (NotSupportedException)
                {
                    //Paths were invalid, move on
                }
                catch (ArgumentException)
                {
                    //Paths were invalid, move on
                }
            }
        }
    }
}
