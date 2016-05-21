using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class TemplatedHtmlRenderer
    {
        HtmlDocument doc;

        public void openDoc(String file)
        {
            doc = new HtmlDocument();
            doc.Load(file);
        }

        public HtmlRenderer getRenderer(HtmlElements element)
        {
            switch(element)
            {
                case HtmlElements.text:
                    return new EncodedHtmlRenderer();
                default:
                    var template = doc.DocumentNode.SelectNodes($"//templates/{element}").FirstOrDefault();
                    if (template == null)
                    {
                        return null;
                    }

                    if (template.GetAttributeValue("exclude", false))
                    {
                        return new SplitHtmlTemplate(template.InnerHtml);
                    }
                    else
                    {
                        return new SplitHtmlTemplate(template.OuterHtml);
                    }
            }
        }

    }
}
