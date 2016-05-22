using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using HtmlAgilityPack;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class CustomHtmlElementRenderer : HtmlElementRenderer
    {
        private String template;

        public CustomHtmlElementRenderer(String template)
        {
            this.template = template;
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode element, out bool ignoreChildNodes)
        {
            String result = template;
            
            foreach(var attr in element.Attributes)
            {
                result = result.Replace("@" + attr.Name, attr.Value);
            }

            htmlFormatter.write(result);

            ignoreChildNodes = true;
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode element, out bool ignoreChildNodes)
        {
            String result = template;

            foreach (var attr in element.Attributes)
            {
                result = result.Replace("@" + attr.Name, attr.Value);
            }

            htmlFormatter.write(result);

            ignoreChildNodes = true;
        }
    }
}
