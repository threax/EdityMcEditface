using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.TemplateFormatter
{
    interface HtmlTagIdentifier
    {
        String identify(Inline inline, bool isOpening, bool isClosing);

        String identify(Block block, bool isOpening, bool isClosing);
    }
}
