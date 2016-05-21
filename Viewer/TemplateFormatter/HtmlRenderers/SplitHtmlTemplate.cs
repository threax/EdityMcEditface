using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class SplitHtmlTemplate : HtmlRenderer
    {
        protected String prefix;
        protected String postfix;
        protected bool ignoreChildNodes;

        public SplitHtmlTemplate(String html, bool ignoreChildNodes = false, char delimiter = '|')
        {
            String[] split = html.Split(new char[]{ delimiter });
            if(split.Length != 2)
            {
                throw new Exception($"invalid split line {html} too many delimiters {delimiter}");
            }

            prefix = split[0];
            postfix = split[1];
            this.ignoreChildNodes = ignoreChildNodes;
        }

        public virtual void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

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

        public virtual void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            ignoreChildNodes = this.ignoreChildNodes;

            if (isOpening)
            {
                htmlFormatter.ensureNewLine();
                htmlFormatter.write(prefix);
            }

            if(isClosing)
            {
                htmlFormatter.write(postfix);
            }
        }
    }
}
