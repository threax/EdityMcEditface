using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer
{
    public enum HtmlElements
    {
        h1, h2, h3, h4, h5, h6,
        blockquote,
        document,
        fencedcode,
        htmlblock,
        indentedcode,
        ul,
        ol,
        li,
        p,
        hr,
        text,
        br,
        softbr,
        code,
        a,
        img,
        strong,
        em,
        del,
        unidentified
    }

    public interface HtmlTagIdentifier
    {
        HtmlElements identify(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter);

        HtmlElements identify(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter);
    }
}
