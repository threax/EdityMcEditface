using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class WriteRenderer : HtmlRenderer
    {
        private bool ignoreChildNodes;

        public WriteRenderer(bool ignoreChildNodes = true)
        {
            this.ignoreChildNodes = ignoreChildNodes;
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.write(block.StringContent);
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.write(inline.LiteralContent);
        }
    }
}
