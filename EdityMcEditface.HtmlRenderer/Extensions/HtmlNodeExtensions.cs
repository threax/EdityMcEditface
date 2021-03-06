﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlAgilityPack
{
    public static class HtmlNodeExtensions
    {
        public static IEnumerable<HtmlNode> Select(this HtmlNode node, String query)
        {
            var prefix = "";
            if(node != node.OwnerDocument.DocumentNode)
            {
                prefix = ".";
            }
            IEnumerable<HtmlNode> result;
            switch (query[0])
            {
                case '[':
                    result = node.SelectNodes($"{prefix}//*[@{query.Substring(1)}");
                    break;
                case '#':
                    result = node.SelectNodes($"{prefix}//*[@id='{query.Substring(1)}']");
                    break;
                case '.':
                    result = node.SelectNodes($"{prefix}//*[@class='{query.Substring(1)}']");
                    break;
                default:
                    if (query.Contains('['))
                    {
                        query = query.Replace("[", "[@");
                    }
                    if (query.Contains('.'))
                    {
                        query = query.Replace(".", "[@class='") + "']";
                    }
                    result = node.SelectNodes($"{prefix}//{query}");
                    break;
            }
            if(result == null)
            {
                result = new HtmlNode[0];
            }
            return result;
        }

        public static void AddClass(this HtmlNode node, String cls)
        {
            var clses = node.GetAttributeValue("class", "");
            if (clses != "")
            {
                clses += " ";
            }
            clses += cls;
            node.SetAttributeValue("class", clses);
        }

        public static HtmlNode FirstElementChild(this HtmlNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                if (child.NodeType == HtmlNodeType.Element)
                {
                    return child;
                }
            }
            return null;
        }
    }
}
