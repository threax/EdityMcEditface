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
        /// An enumerator over pages. The first item should be the innermost page.
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
            for(int i = 0; i < pageDefinitions.Count; ++i)
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
                    }
                }
                innerHtml = templateContent.Replace("{mainContent}", innerHtml);
            }

            //Build variables up
            environment.buildVariables(pageDefinitions);

            String formattedText = formatText(innerHtml);

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

        private String formatText(String text)
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
                                var variable = text.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
                                String value;
                                if (variable[0] == '|') //Starts with a pipe, pass it to the client side without the pipe.
                                {
                                    value = $"{{{variable.Substring(1)}}}";
                                }
                                else
                                {
                                    value = environment.getVariable(variable, "");
                                }
                                output.Append(text.Substring(textStart, bracketStart - textStart));
                                if (environment.encodeOutput(variable))
                                {
                                    value = System.Net.WebUtility.HtmlEncode(value);
                                }
                                output.Append(value);
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
