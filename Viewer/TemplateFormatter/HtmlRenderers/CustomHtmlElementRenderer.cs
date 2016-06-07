using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using HtmlAgilityPack;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class CustomHtmlElementRenderer : HtmlElementRenderer
    {
        private String template;

        public CustomHtmlElementRenderer(String template)
        {
            this.template = template;
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode element)
        {
            writeNode(htmlFormatter, element);
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode element)
        {
            writeNode(htmlFormatter, element);
        }

        public void writeNode(AccessibleHtmlFormatter htmlFormatter, HtmlNode element)
        {
            int lastBracket = 0;
            for (int i = 0; i < template.Length;)
            {
                int bracketIndex = template.IndexOf('{', i);
                if (bracketIndex != -1 && bracketIndex + 1 < template.Length && template[bracketIndex + 1] != '}')
                {
                    htmlFormatter.write(template.Substring(lastBracket, bracketIndex - lastBracket));
                    int closeIndex = template.IndexOf('}', bracketIndex);
                    if (closeIndex != -1)
                    {
                        String varName = template.Substring(bracketIndex + 1, closeIndex - bracketIndex - 1);
                        if (varName == "content")
                        {
                            htmlFormatter.write(element.InnerHtml);
                        }
                        else
                        {
                            htmlFormatter.write(element.GetAttributeValue(varName, ""));
                        }

                        i = lastBracket = closeIndex + 1;
                    }
                    else
                    {
                        lastBracket = bracketIndex;
                        i = template.Length;
                    }
                }
                else
                {
                    i = template.Length;
                }
            }

            htmlFormatter.write(template.Substring(lastBracket, template.Length - lastBracket));
        }
    }
}
