using CommonMark.Syntax;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class RawWriteHtmlElementRenderer : HtmlElementRenderer
    {
        public RawWriteHtmlElementRenderer()
        {

        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode elementNode)
        {
            htmlFormatter.write(block.StringContent);
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode elementNode)
        {
            htmlFormatter.write(inline.LiteralContent);
        }
    }
}
