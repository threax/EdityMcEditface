using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public interface ServerSideTransform
    {
        void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions);
    }
}
