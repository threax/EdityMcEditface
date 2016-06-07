using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using HtmlAgilityPack;

namespace CommonMarkTools.Renderer.HtmlRenderers
{
    class DocumentRenderer : HtmlRenderer
    {
        private String prefix;
        private String postfix;

        public DocumentRenderer(String templateHtml, TemplateEnvironment environment)
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

            //Replace variables in template, note that this could be way better than this version
            //This creates a ton of stings and is not good code, but will get this up and running quickly
            //TODO: QuickFix - Make the environment variables for templates more efficient
            foreach(var variable in environment.Variables)
            {
                prefix = prefix.Replace("{" + variable.Key + "}", variable.Value);
                postfix = postfix.Replace("{" + variable.Key + "}", variable.Value);
            }
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
