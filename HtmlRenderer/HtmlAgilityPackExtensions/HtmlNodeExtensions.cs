using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlAgilityPack
{
    public static class HtmlNodeExtensions
    {
        public static HtmlNodeCollection Select(this HtmlNode node, String query)
        {
            switch (query[0])
            {
                case '[':
                    return node.SelectNodes($"//*[@{query.Substring(1)}");
                case '#':
                    return node.SelectNodes($"//*[@id='{query.Substring(1)}']");
                case '.':
                    return node.SelectNodes($"//*[@class='{query.Substring(1)}']");
                default:
                    if (query.Contains('['))
                    {
                        query = query.Replace("[", "[@");
                    }
                    if (query.Contains('.'))
                    {
                        query = query.Replace(".", "[@class='") + "']";
                    }
                    return node.SelectNodes("//" + query);
            }
        }
    }
}
