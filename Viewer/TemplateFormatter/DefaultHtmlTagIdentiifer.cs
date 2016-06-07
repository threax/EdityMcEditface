using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace CommonMarkTools.Renderer
{
    class DefaultHtmlTagIdentiifer : HtmlTagIdentifier
    {
        public HtmlElements identify(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter)
        {
            switch (block.Tag)
            {
                case BlockTag.AtxHeading:
                case BlockTag.SetextHeading:
                    switch (block.Heading.Level)
                    {
                        case 1:
                            return HtmlElements.h1;
                        case 2:
                            return HtmlElements.h2;
                        case 3:
                            return HtmlElements.h3;
                        case 4:
                            return HtmlElements.h4;
                        case 5:
                            return HtmlElements.h5;
                        case 6:
                            return HtmlElements.h6;
                        default:
                            return HtmlElements.p;
                    }
                case BlockTag.BlockQuote:
                    if (isOpening)
                    {
                        htmlFormatter.TheRenderTightParagraphs.Push(false);
                    }

                    if (isClosing)
                    {
                        htmlFormatter.TheRenderTightParagraphs.Pop();
                    }
                    return HtmlElements.blockquote;
                case BlockTag.Document:
                    return HtmlElements.document;
                case BlockTag.FencedCode:
                    return HtmlElements.fencedcode;
                case BlockTag.HtmlBlock:
                    return HtmlElements.htmlblock;
                case BlockTag.IndentedCode:
                    return HtmlElements.indentedcode;
                case BlockTag.List:
                    if (isOpening)
                    {
                        htmlFormatter.TheRenderTightParagraphs.Push(block.ListData.IsTight);
                    }

                    if (isClosing)
                    {
                        htmlFormatter.TheRenderTightParagraphs.Pop();
                    }

                    switch (block.ListData.ListType)
                    {
                        default:
                        case ListType.Bullet:
                            return HtmlElements.ul;
                        case ListType.Ordered:
                            return HtmlElements.ol;
                    }
                case BlockTag.ListItem:
                    return HtmlElements.li;
                case BlockTag.Paragraph:
                    if (htmlFormatter.TheRenderTightParagraphs.Peek())
                    {
                        return HtmlElements.unidentified;
                    }
                    return HtmlElements.p;
                case BlockTag.ThematicBreak:
                    return HtmlElements.hr;
                case BlockTag.ReferenceDefinition:
                default:
                    return HtmlElements.unidentified;
            }
        }

        public HtmlElements identify(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter)
        {
            switch (inline.Tag)
            {
                case InlineTag.RawHtml:
                    return HtmlElements.htmlblock;
                case InlineTag.String:
                    return HtmlElements.text;
                case InlineTag.LineBreak:
                    return HtmlElements.br;
                case InlineTag.SoftBreak:
                    return HtmlElements.softbr;
                case InlineTag.Code:
                    return HtmlElements.code;
                case InlineTag.Link:
                    return HtmlElements.a;
                case InlineTag.Image:
                    return HtmlElements.img;
                case InlineTag.Strong:
                    return HtmlElements.strong;
                case InlineTag.Emphasis:
                    return HtmlElements.em;
                case InlineTag.Strikethrough:
                    return HtmlElements.del;
                default:
                    return HtmlElements.unidentified;
            }
        }
    }
}
