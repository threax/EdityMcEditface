using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public static class HtmlrestQueries
    {
        public static HtmlNode getControllerNode(String name, HtmlNode context)
        {
            return context.Select($"[data-hr-controller='{name}']").FirstOrDefault();
        }

        public static HtmlNode getModelNode(String name, HtmlNode context)
        {
            return context.Select($"[data-hr-model='{name}']").FirstOrDefault();
        }

        public static HtmlNode getModelTemplate(HtmlNode modelNode)
        {
            var componentName = modelNode.GetAttributeValue("data-hr-model-component", (string)null);
            if (componentName != null)
            {
                return modelNode.OwnerDocument.DocumentNode.Select($"[data-hr-component='{componentName}']").FirstOrDefault();
            }
            else
            {
                return modelNode.Select("template").FirstOrDefault();
            }
        }

        public static String getModelSrc(HtmlNode node)
        {
            return node.GetAttributeValue("data-hr-model-src", (String)null);
        }
    }
}
