using Edity.McEditface.HtmlRenderer;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class HtmlDocumentRenderer
    {
        private TemplateEnvironment environment;
        private List<ServerSideTransform> transforms = new List<ServerSideTransform>();

        public HtmlDocumentRenderer(TemplateEnvironment environment)
        {
            this.environment = environment;
        }

        public void addTransform(ServerSideTransform transform)
        {
            this.transforms.Add(transform);
        }

        /// <summary>
        /// Build the document for the given page stack.
        /// </summary>
        /// <param name="pageStack">The pages, the first item should be the innermost page.</param>
        /// <returns></returns>
        public HtmlDocument getDocument(IEnumerable<PageStackItem> pageStack)
        {
            HtmlDocument document = new HtmlDocument();
            //Replace main content first then main replace will get its variables
            //Not the best algo
            String innerHtml = null;
            List<PageStackItem> pageDefinitions = new List<PageStackItem>(pageStack);
            if (pageDefinitions.Count > 0)
            {
                innerHtml = pageDefinitions[0].Content;
            }
            int last = pageDefinitions.Count - 1;
            for(int i = 1; i < pageDefinitions.Count; ++i)
            {
                var templateContent = pageDefinitions[i].Content;
                if (i != last && templateContent.StartsWith("<!doctype", StringComparison.OrdinalIgnoreCase))
                {
                    //Not the last template with an html tag, remove it and only take the body
                    document.LoadHtml(templateContent);
                    var body = document.DocumentNode.Select("body").FirstOrDefault();
                    if (body != null)
                    {
                        templateContent = body.InnerHtml;
                        templateContent = templateContent.Replace("{javascript}", "");
                        templateContent = templateContent.Replace("{css}", "");
                    }
                }
                innerHtml = templateContent.Replace("{mainContent}", innerHtml);
            }

            //Build variables up
            environment.buildVariables(pageDefinitions);

            String formattedText = TextFormatter.formatText(innerHtml, environment, WebUtility.HtmlEncode);

            document.LoadHtml(formattedText);
            //Run transforms
            foreach (var transform in transforms)
            {
                transform.transform(document);
            }

            return document;
        }

        public ICollection<ServerSideTransform> Transforms
        {
            get
            {
                return transforms;
            }
        }
    }
}
