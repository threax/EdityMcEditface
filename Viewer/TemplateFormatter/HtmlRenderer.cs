using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer
{
    interface HtmlRenderer
    {
        void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes);

        void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes);
    }
}
