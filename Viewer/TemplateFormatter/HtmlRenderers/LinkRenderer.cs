using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class LinkRenderer : SplitHtmlTemplate
    {
        private String beforeHref;
        private String afterHref;

        public LinkRenderer(String html, bool ignoreChildNodes = false, char delimiter = '|')
            :base(html, ignoreChildNodes, delimiter)
        {
            var split = prefix.Split(new String[] { "href=\"\"" }, StringSplitOptions.RemoveEmptyEntries);
            if(split.Length != 2)
            {
                throw new Exception($"Invalid link template {html} cannot split on href=\"\"");
            }

            this.beforeHref = split[0];
            this.afterHref = split[1];
        }

        public override void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;
            if(isOpening)
            {
                htmlFormatter.write($"{beforeHref} href=\"");
                var uriResolver = htmlFormatter.TheSettings.UriResolver;
                if (uriResolver != null)
                {
                    htmlFormatter.writeEncodedUrl(uriResolver(inline.TargetUrl));
                }
                else
                {
                    htmlFormatter.writeEncodedUrl(inline.TargetUrl);
                }


                htmlFormatter.write('\"');
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

                htmlFormatter.write(afterHref);
            }
            if(isClosing)
            {
                base.write(inline, isOpening, isClosing, htmlFormatter, out ignoreChildNodes);
            }
        }
    }
}
