using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class LinkRenderer : SplitHtmlTemplate
    {
        public LinkRenderer(String html, bool ignoreChildNodes = false, String delimiter = "|")
            :base(html, ignoreChildNodes, delimiter)
        {
            if(prefix.EndsWith(">"))
            {
                prefix = prefix.Substring(0, prefix.Length - 1);
            }
        }

        public override void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;
            if(isOpening)
            {
                htmlFormatter.write($"{prefix} href=\"");
                var uriResolver = htmlFormatter.TheSettings.UriResolver;
                if (uriResolver != null)
                {
                    htmlFormatter.writeEncodedUrl(uriResolver(inline.TargetUrl));
                }
                else
                {
                    htmlFormatter.writeEncodedUrl(inline.TargetUrl);
                }


                htmlFormatter.write("\"");
                if (inline.LiteralContent.Length > 0)
                {
                    htmlFormatter.write(" title=\"");
                    htmlFormatter.writeEncodedHtml(inline.LiteralContent);
                    htmlFormatter.write('\"');
                }

                if (htmlFormatter.TheSettings.TrackSourcePosition)
                {
                    htmlFormatter.writePositionAttribute(inline);
                }

                htmlFormatter.write('>');
            }
            if(isClosing)
            {
                base.write(inline, isOpening, isClosing, htmlFormatter, out ignoreChildNodes);
            }
        }
    }
}
