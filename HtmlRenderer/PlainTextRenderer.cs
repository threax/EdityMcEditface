using Edity.McEditface.HtmlRenderer;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PlainTextRenderer
    {
        private TemplateEnvironment environment;
        private char openingDelimiter;
        private char closingDelimiter;

        public PlainTextRenderer(TemplateEnvironment environment, char openingDelimeter = '{', char closingDelimeter = '}')
        {
            this.environment = environment;
            this.openingDelimiter = openingDelimeter;
            this.closingDelimiter = closingDelimeter;
        }

        /// <summary>
        /// An enumerator over pages. The first item should be the innermost page.
        /// </summary>
        /// <param name="pageStack">The pages, the first item should be the innermost page.</param>
        /// <returns></returns>
        public String getDocument(IEnumerable<PageStackItem> pageStack)
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
            for(int i = 0; i < pageDefinitions.Count; ++i)
            {
                var templateContent = pageDefinitions[i].Content;
                innerHtml = templateContent.Replace("{mainContent}", innerHtml);
            }

            //Build variables up
            environment.buildVariables(pageDefinitions);

            return TextFormatter.formatText(innerHtml, environment);
        }
    }
}
