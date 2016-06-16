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
    public class DocumentRenderer
    {
        private TemplateEnvironment environment;
        private Stack<PageStackItem> pageStack = new Stack<PageStackItem>();
        private List<ServerSideTransform> transforms = new List<ServerSideTransform>();

        public DocumentRenderer(TemplateEnvironment environment)
        {
            this.environment = environment;
        }

        public void pushTemplate(PageStackItem layout)
        {
            pageStack.Push(layout);
        }

        public HtmlDocument getDocument(PageStackItem page)
        {
            HtmlDocument document = new HtmlDocument();
            //Replace main content first then main replace will get its variables
            //Not the best algo
            List<PageStackItem> pageDefinitions = new List<PageStackItem>(pageStack.Count + 1);
            pageDefinitions.Add(page);
            String innerHtml = page.Content;
            while(pageStack.Count > 0)
            {
                var template = pageStack.Pop();
                var templateContent = template.Content;
                if(templateContent.StartsWith("<!doctype", StringComparison.OrdinalIgnoreCase) && pageStack.Count > 0)
                {
                    //Not the last template with an html tag, remove it and only take the body
                    document.LoadHtml(templateContent);
                    var body = document.DocumentNode.Select("body").FirstOrDefault();
                    if(body != null)
                    {
                        templateContent = body.InnerHtml;
                    }
                }
                innerHtml = templateContent.Replace("{mainContent}", innerHtml);
                pageDefinitions.Add(template);
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
