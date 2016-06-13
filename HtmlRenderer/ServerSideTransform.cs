using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edity.McEditface.HtmlRenderer
{
    public interface ServerSideTransform
    {
        String transform(HtmlDocument document);
    }
}
