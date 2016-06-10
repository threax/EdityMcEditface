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
            //Replace main content first then main replace will get its variables
            return formatText(templateHtml.Replace("{MainContent}", innerHtml));
        }

        public string formatText(String text)
        {
            StringBuilder output = new StringBuilder(text.Length);
            var textStart = 0;
            var bracketStart = 0;
            var bracketEnd = 0;
            for (var i = 0; i < text.Length; ++i)
            {
                switch (text[i])
                {
                    case '{':
                        if (text[i + 1] != '{')
                        {
                            bracketStart = i;
                        }
                        break;
                    case '}':
                        if (i + 1 == text.Length || text[i + 1] != '}')
                        {
                            bracketEnd = i;

                            if (bracketStart < bracketEnd - 1)
                            {
                                var value = environment.getVariable(text.Substring(bracketStart + 1, bracketEnd - bracketStart - 1), "");
                                output.Append(text.Substring(textStart, bracketStart - textStart));
                                output.Append(System.Net.WebUtility.HtmlEncode(value));
                                textStart = i + 1;
                            }
                        }
                        break;
                }
            }

            if (textStart < text.Length)
            {
                output.Append(text.Substring(textStart, text.Length - textStart));
            }
            return output.ToString();
        }
    }
}
