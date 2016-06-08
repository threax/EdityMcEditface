using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edity.McEditface.HtmlRenderer
{
    public class DocumentRenderer
    {
        private TemplateEnvironment environment;
        private String templateHtml;

        public DocumentRenderer(String templateHtml, TemplateEnvironment environment)
        {
            this.templateHtml = templateHtml;
            this.environment = environment;
        }

        public string getDocument(String innerHtml)
        {
            innerHtml = templateHtml.Replace("{MainContent}", innerHtml);
            //Replace variables in template, note that this could be way better than this version
            //This creates a ton of stings and is not good code, but will get this up and running quickly
            //TODO: QuickFix - Make the environment variables for templates more efficient
            foreach (var variable in environment.Variables)
            {
                innerHtml = innerHtml.Replace("{" + variable.Key + "}", variable.Value);
            }

            return innerHtml;
        }
    }
}
