using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class EncodedHtmlRenderer : HtmlRenderer
    {
        private bool ignoreChildNodes;

        public EncodedHtmlRenderer(bool ignoreChildNodes = true)
        {
            this.ignoreChildNodes = ignoreChildNodes;
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.writeEncodedHtml(block.StringContent);
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.writeEncodedHtml(inline.LiteralContent);
        }
    }
}
