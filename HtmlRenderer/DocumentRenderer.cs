﻿using Edity.McEditface.HtmlRenderer;
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
        private Stack<String> templateStack = new Stack<string>();
        private List<ServerSideTransform> transforms = new List<ServerSideTransform>();

        public DocumentRenderer(TemplateEnvironment environment)
        {
            this.environment = environment;
        }

        public void pushTemplate(String template)
        {
            templateStack.Push(template);
        }

        public String getDocument(String innerHtml, PageDefinition pageSettings)
        {
            using (var stream = new MemoryStream(innerHtml.Length * sizeof(char)))
            {
                getDocument(innerHtml, stream, pageSettings);
                stream.Position = 0;
                using(var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public void getDocument(String innerHtml, Stream outStream, PageDefinition pageSettings)
        {
            //Build variables up
            environment.buildVariables(pageSettings);

            //Replace main content first then main replace will get its variables
            //Not the best algo
            while(templateStack.Count > 0)
            {
                String template = templateStack.Pop();
                innerHtml = template.Replace("{mainContent}", innerHtml);
            }
            String sb = formatText(innerHtml);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(sb);
            //Run transforms
            foreach (var transform in transforms)
            {
                transform.transform(document);
            }

            document.Save(outStream);
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
