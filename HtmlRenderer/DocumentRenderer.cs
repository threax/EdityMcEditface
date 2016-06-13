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
        private String templateHtml;
        private List<ServerSideTransform> transforms = new List<ServerSideTransform>();

        public DocumentRenderer(String templateHtml, TemplateEnvironment environment)
        {
            this.templateHtml = templateHtml;
            this.environment = environment;
        }

        public String getDocument(String innerHtml)
        {
            using (var stream = new MemoryStream(innerHtml.Length * sizeof(char)))
            {
                getDocument(innerHtml, stream);
                stream.Position = 0;
                using(var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public void getDocument(String innerHtml, Stream outStream)
        {
            //Replace main content first then main replace will get its variables
            var withContent = templateHtml.Replace("{mainContent}", innerHtml);
            String sb = formatText(withContent);

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
                                var value = environment.getVariable(variable, "");
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
