using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class ImageHtmlRenderer : SplitHtmlTemplate
    {
        public ImageHtmlRenderer(String html, bool ignoreChildNodes = false)
            :base(html, ignoreChildNodes, "<img />")
        {
        }

        public override void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            if(isOpening)
            {
                htmlFormatter.write(prefix);
                htmlFormatter.write("<img src=\"");
                var uriResolver = htmlFormatter.TheSettings.UriResolver;
                if (uriResolver != null)
                {
                    htmlFormatter.writeEncodedUrl(uriResolver(inline.TargetUrl));
                }
                else
                {
                    htmlFormatter.writeEncodedUrl(inline.TargetUrl);
                }

                htmlFormatter.write("\" alt=\"");

                htmlFormatter.TheRenderPlainTextInlines.Push(true);
            }

            if (isClosing)
            {
                htmlFormatter.TheRenderPlainTextInlines.Pop();

                htmlFormatter.write('\"');
                if (inline.LiteralContent.Length > 0)
                {
                    htmlFormatter.write(" title=\"");
                    htmlFormatter.writeEncodedHtml(inline.LiteralContent);
                    htmlFormatter.write('\"');
                }

                htmlFormatter.write(" />");
                htmlFormatter.write(postfix);
            }
        }
    }
}
