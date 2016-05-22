using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using HtmlAgilityPack;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class DocumentRenderer : HtmlRenderer
    {
        private String prefix;
        private String postfix;

        public DocumentRenderer(String templateHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(templateHtml);
            var template = doc.DocumentNode.SelectNodes($"//templates").FirstOrDefault();
            if(template == null)
            {
                throw new Exception("Invalid template document. Missing templates element.");
            }

            template.ParentNode.InsertAfter(doc.CreateTextNode("SPLIT_HERE"), template);
            template.Remove();

            var htmlTemplates = doc.DocumentNode.SelectNodes($"//htmltemplates");
            if(htmlTemplates != null)
            {
                foreach(var tempate in htmlTemplates)
                {
                    tempate.Remove();
                }
            }

            var html = doc.DocumentNode.OuterHtml;
            String[] split = html.Split(new String[] { "SPLIT_HERE" }, StringSplitOptions.None);
            prefix = split[0];
            postfix = split[1];
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;

            if (isOpening)
            {
                htmlFormatter.ensureNewLine();
                htmlFormatter.write(prefix);
            }

            if (isClosing)
            {
                htmlFormatter.write(postfix);
            }
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;
        }
    }
}
