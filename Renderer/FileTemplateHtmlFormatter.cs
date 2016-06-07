using CommonMark;
using CommonMark.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace CommonMarkTools.Renderer
{
    class FileTemplateHtmlFormatter : AccessibleHtmlFormatter
    {
        HtmlTagMap tagMap;
        HtmlTagIdentifier tagIdentifier;

        public FileTemplateHtmlFormatter(HtmlTagMap tagMap, HtmlTagIdentifier tagIdentifier, TextWriter target, CommonMarkSettings settings)
            :base(target, settings)
        {
            this.tagMap = tagMap;
            this.tagIdentifier = tagIdentifier;
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;
            bool runDefault = true;
            if(!this.RenderPlainTextInlines.Peek())
            {
                HtmlElements tag = tagIdentifier.identify(inline, isOpening, isClosing, this);
                HtmlRenderer renderer;
                if(tag != HtmlElements.unidentified && tagMap.tryGetTag(tag, out renderer))
                {
                    runDefault = false;
                    renderer.write(inline, isOpening, isClosing, this, out ignoreChildNodes);
                }
            }

            if(runDefault)
            {
                base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
            }
        }

        protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;
            bool runDefault = true;
            HtmlElements tag = tagIdentifier.identify(block, isOpening, isClosing, this);
            HtmlRenderer renderer;
            if (tag != HtmlElements.unidentified && tagMap.tryGetTag(tag, out renderer))
            {
                runDefault = false;
                renderer.write(block, isOpening, isClosing, this, out ignoreChildNodes);
            }

            if (runDefault)
            {
                base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
            }
        }
    }
}
