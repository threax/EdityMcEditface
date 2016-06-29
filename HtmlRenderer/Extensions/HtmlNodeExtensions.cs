using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlAgilityPack
{
    public static class HtmlNodeExtensions
    {
        public static IEnumerable<HtmlNode> Select(this HtmlNode node, String query)
        {
            IEnumerable<HtmlNode> result;
            switch (query[0])
            {
                case '[':
                    result = node.SelectNodes($"//*[@{query.Substring(1)}");
                    break;
                case '#':
                    result = node.SelectNodes($"//*[@id='{query.Substring(1)}']");
                    break;
                case '.':
                    result = node.SelectNodes($"//*[@class='{query.Substring(1)}']");
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
                    result = node.SelectNodes("//" + query);
                    break;
            }
            if(result == null)
            {
                result = new HtmlNode[0];
            }
            return result;
        }
    }
}
