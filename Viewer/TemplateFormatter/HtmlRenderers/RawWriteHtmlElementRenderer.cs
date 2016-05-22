using CommonMark.Syntax;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class RawWriteHtmlElementRenderer : HtmlElementRenderer
    {
        private bool ignoreChildNodes;

        public RawWriteHtmlElementRenderer(bool ignoreChildNodes = true)
        {
            this.ignoreChildNodes = ignoreChildNodes;
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode elementNode, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.write(block.StringContent);
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode elementNode, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.write(inline.LiteralContent);
        }
    }
}
