using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class LiteralHtmlTemplate : HtmlRenderer
    {
        private String html;
        private bool ignoreChildNodes;

        public LiteralHtmlTemplate(String html, bool ignoreChildNodes = true)
        {
            this.html = html;
            this.ignoreChildNodes = ignoreChildNodes;
        }

        public virtual void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;
            htmlFormatter.ensureNewLine();
            htmlFormatter.write(html);
        }

        public virtual void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;
            htmlFormatter.ensureNewLine();
            htmlFormatter.write(html);
        }
    }
}
