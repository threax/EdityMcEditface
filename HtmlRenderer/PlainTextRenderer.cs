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
        /// Build the document for the given page stack.
        /// </summary>
        /// <param name="pageStack">The pages, the first item should be the innermost page.</param>
        /// <returns></returns>
        public String getDocument(IEnumerable<PageStackItem> pageStack)
        {
            String mainContentVar = $"{openingDelimiter}mainContent{closingDelimiter}";
            String innerHtml = "";// = pageStack.First().Content;
            foreach (var page in pageStack)//.Skip(1))
            {
                innerHtml = page.Content.Replace(mainContentVar, innerHtml);
            }
            environment.buildVariables(pageStack);

            return TextFormatter.formatText(innerHtml, environment, openingDelimiter, closingDelimiter);
        }
    }
}
