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
        private HtmlDocument doc;
        private DocumentRenderer docRenderer;

        public void openDoc(String file)
        {
            doc = new HtmlDocument();
            doc.Load(file);
            docRenderer = new DocumentRenderer(doc.DocumentNode.OuterHtml);
        }

        public HtmlRenderer getRenderer(HtmlElements element)
        {
            switch(element)
            {
                case HtmlElements.document:
                    return docRenderer;
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
                case HtmlElements.fencedcode:
                case HtmlElements.indentedcode:
                    template = doc.DocumentNode.SelectNodes($"//templates/{element}").FirstOrDefault();
                    if (template == null)
                    {
                        return null;
                    }

                    return new CodeRenderer(this.getNodeHtml(template));
                case HtmlElements.img:
                    template = doc.DocumentNode.SelectNodes($"//templates/{element}").FirstOrDefault();
                    if (template == null)
                    {
                        return null;
                    }

                    return new ImageHtmlRenderer(this.getNodeHtml(template));
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
