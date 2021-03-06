﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public static class HtmlRapierQueries
    {
        public static HtmlNode getControllerNode(String name, HtmlNode context)
        {
            return context.Select($"[data-hr-controller='{name}']").FirstOrDefault();
        }

        public static HtmlNode getModelNode(String name, HtmlNode context)
        {
            return context.Select($"[data-hr-model='{name}']").FirstOrDefault();
        }

        public static HtmlNode getModelTemplate(HtmlNode modelNode, String variant = null)
        {
            IEnumerable<HtmlNode> query;
            var componentName = modelNode.GetAttributeValue("data-hr-model-component", (string)null);
            if (componentName != null)
            {
                query = modelNode.OwnerDocument.DocumentNode.Select($"[data-hr-component='{componentName}']");
            }
            else
            {
                query = modelNode.Select("template");
            }

            var current = query.FirstOrDefault();

            if (variant != null)
            {
                //Check current node and all siblings for variant
                while (current != null)
                {
                    var attr = current.GetAttributeValue("data-hr-variant", (String)null);
                    if (attr == variant)
                    {
                        return current;
                    }
                    current = current.NextSibling;
                }

                //If nothing found return the current, this is what htmlrapier does
            }

            return current;
        }

        public static String getModelSrc(HtmlNode node)
        {
            return node.GetAttributeValue("data-hr-model-src", (String)null);
        }
    }
}
