using CommonMark.Syntax;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.TemplateFormatter.HtmlRenderers
{
    class HtmlRendererFinder : HtmlRenderer
    {
        private bool ignoreChildNodes;
        private TemplatedHtmlRenderer templateRenderer;
        private Dictionary<String, HtmlElementRenderer> elementRenderers = new Dictionary<string, HtmlElementRenderer>();

        public HtmlRendererFinder(TemplatedHtmlRenderer templateRenderer, bool ignoreChildNodes = true)
        {
            this.ignoreChildNodes = ignoreChildNodes;
            this.templateRenderer = templateRenderer;
        }

        public void write(Block block, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(block.StringContent.ToString());
            var node = doc.DocumentNode.FirstChild;
            getRenderer(node).write(block, isOpening, isClosing, htmlFormatter, node, out ignoreChildNodes);
        }

        public void write(Inline inline, bool isOpening, bool isClosing, AccessibleHtmlFormatter htmlFormatter, out bool ignoreChildNodes)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(inline.LiteralContent);
            var node = doc.DocumentNode.FirstChild;
            getRenderer(node).write(inline, isOpening, isClosing, htmlFormatter, node, out ignoreChildNodes);
        }

        private HtmlElementRenderer getRenderer(HtmlNode docNode)
        {
            String name = "__reserved_unknown_default";
            if(docNode != null)
            {
                name = docNode.Name;
            }
            HtmlElementRenderer renderer;
            if (!elementRenderers.TryGetValue(name, out renderer))
            {
                renderer = templateRenderer.getHtmlTemplateElementRenderer(name);
                elementRenderers.Add(name, renderer);
            }

            return renderer;
        }
    }
}
