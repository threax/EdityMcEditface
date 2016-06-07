using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class CodeRenderer : SplitHtmlTemplate
    {
        public CodeRenderer(String html, bool ignoreChildNodes = true, String delimiter = "|")
            :base(html, ignoreChildNodes, delimiter)
        {

        }

        public override void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.ensureNewLine();
            htmlFormatter.write(prefix);
            htmlFormatter.writeEncodedHtml(block.StringContent);
            htmlFormatter.write(postfix);
        }

        public override void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            htmlFormatter.ensureNewLine();
            htmlFormatter.write(prefix);
            htmlFormatter.writeEncodedHtml(inline.LiteralContent);
            htmlFormatter.write(postfix);
        }
    }
}
