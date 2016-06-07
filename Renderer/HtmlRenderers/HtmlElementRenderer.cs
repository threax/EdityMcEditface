using CommonMark.Syntax;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    public interface HtmlElementRenderer
    {
        void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode element);

        void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, HtmlNode element);
    }
}
