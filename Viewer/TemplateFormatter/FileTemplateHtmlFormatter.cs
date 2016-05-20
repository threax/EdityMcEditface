using CommonMark;
using CommonMark.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace Viewer.TemplateFormatter
{
    class FileTemplateHtmlFormatter : HtmlFormatter
    {
        TagCache<InlineTag> inlineTags;
        TagCache<BlockTag> blockTags;

        public FileTemplateHtmlFormatter(String templateDirectory, TextWriter target, CommonMarkSettings settings)
            :base(target, settings)
        {
            inlineTags = new TagCache<InlineTag>(templateDirectory);
            blockTags = new TagCache<BlockTag>(templateDirectory);
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;
            bool runDefault = true;
            if(!this.RenderPlainTextInlines.Peek())
            {
                String tag;
                if(inlineTags.tryGetTag(inline.Tag, out tag))
                {
                    runDefault = false;

                    if(isOpening)
                    {

                    }

                    if(isClosing)
                    {

                    }
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
            if (!this.RenderPlainTextInlines.Peek())
            {
                String tag;
                if(blockTags.tryGetTag(block.Tag, out tag))
                {
                    runDefault = false;

                    if (isOpening)
                    {

                    }

                    if (isClosing)
                    {

                    }
                }
            }

            if (runDefault)
            {
                base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
            }
        }
    }
}
