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
                case HtmlElements.document:
                    return null;
                case HtmlElements.text:
                    return new EncodedHtmlRenderer();
                case HtmlElements.htmlblock:
                    return new WriteRenderer();
                case HtmlElements.br:
                case HtmlElements.hr:
                    var template = doc.DocumentNode.SelectNodes($"//templates/{element}").FirstOrDefault();
                    if (template == null)
                    {
                        return null;
                    }

                    return new LiteralHtmlTemplate(this.getNodeHtml(template));
                case HtmlElements.a:
                    template = doc.DocumentNode.SelectNodes($"//templates/{element}").FirstOrDefault();
                    if (template == null)
                    {
                        return null;
                    }

                    return new LinkRenderer(this.getNodeHtml(template));
                default:
                    template = doc.DocumentNode.SelectNodes($"//templates/{element}").FirstOrDefault();
                    if (template == null)
                    {
                        return null;
                    }

                    return new SplitHtmlTemplate(this.getNodeHtml(template));
            }
        }

        private String getNodeHtml(HtmlNode template)
        {
            if (template.GetAttributeValue("exclude", false))
            {
                return template.InnerHtml;
            }
            else
            {
                return template.OuterHtml;
            }
        }
    }
}
