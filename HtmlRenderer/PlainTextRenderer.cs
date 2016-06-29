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
        private Func<String, String> escapeFunc;

        public PlainTextRenderer(TemplateEnvironment environment, Func<String, String> escapeFunc, char openingDelimeter = '{', char closingDelimeter = '}')
        {
            this.environment = environment;
            this.openingDelimiter = openingDelimeter;
            this.closingDelimiter = closingDelimeter;
            this.escapeFunc = escapeFunc;
        }

        /// <summary>
        /// Build the document for the given page stack.
        /// </summary>
        /// <param name="pageStack">The pages, the first item should be the innermost page.</param>
        /// <returns></returns>
        public String getDocument(IEnumerable<PageStackItem> pageStack)
        {
            String mainContentVar = $"{openingDelimiter}mainContent{closingDelimiter}";
            String innerHtml = "";
            foreach (var page in pageStack)
            {
                innerHtml = page.Content.Replace(mainContentVar, innerHtml);
            }
            environment.buildVariables(pageStack);

            return TextFormatter.formatText(innerHtml, environment, escapeFunc, openingDelimiter, closingDelimiter);
        }
    }
}
