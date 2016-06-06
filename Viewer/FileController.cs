using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Viewer.TemplateFormatter;
using Viewer.TemplateFormatter.HtmlRenderers;

namespace Viewer
{
    public class FileController : ApiController
    {
        private static char[] seps = { '|' };
        private static Assembly assembly = Assembly.GetExecutingAssembly();

        public HttpResponseMessage Get(String file)
        {
            if (file.EndsWith(".edit", StringComparison.InvariantCultureIgnoreCase))
            {

            }

            bool parse = true;
            String extension = Path.GetExtension(file).ToLowerInvariant().Replace(".", "");
            Tuple<String, String> template = null;

            if(isTemplate(extension))
            {
                template = loadEmbeddedTemplate(extension);
                file = file.Substring(0, file.Length - extension.Length - 1) + ".md";
                parse = false;
            }
            else if (!Path.GetExtension(file).Equals(".md", StringComparison.InvariantCultureIgnoreCase))
            {
                var autoFile = file + ".md";
                if (File.Exists(autoFile))
                {
                    file = autoFile;
                }
                else
                {
                    return statusCodeResponse(HttpStatusCode.NotFound);
                }
            }

            var identifier = new DefaultHtmlTagIdentiifer();
            var renderers = new TemplatedHtmlRenderer();
            renderers.openDoc("template.html");
            var tagMap = new HtmlTagMap(renderers.getRenderer);
            CommonMarkSettings.Default.OutputDelegate = (doc, output, settings) => new FileTemplateHtmlFormatter(tagMap, identifier, output, settings).WriteDocument(doc);

            try
            {
                String content;
                using (var reader = new StreamReader(file))
                {
                    if(parse)
                    {
                        using (var writer = new StringWriter())
                        {
                            CommonMarkConverter.Convert(reader, writer);
                            content = writer.ToString();
                        }
                    }
                    else
                    {
                        content = template.Item1 + HttpUtility.HtmlEncode(reader.ReadToEnd()) + template.Item2;
                    }
                }

                return htmlResponse(content);
            }
            catch(FileNotFoundException)
            {
                return statusCodeResponse(HttpStatusCode.NotFound);
            }
            catch(Exception)
            {
                return statusCodeResponse(HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage statusCodeResponse(HttpStatusCode code)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = code;
            return response;
        }

        public HttpResponseMessage htmlResponse(String content, String mimeType = "text/html")
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return response;
        }

        public Tuple<String, String> loadEmbeddedTemplate(String name)
        {
            String template;
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(makeTemplateName(name))))
            {
                template = reader.ReadToEnd();
            }
            var split = template.Split(seps);
            if(split.Length != 2)
            {
                throw new Exception($"Invalid template {name}, split length was {split.Length} should be 2");
            }
            return Tuple.Create(split[0], split[1]);
        }

        private bool isTemplate(String name)
        {
            return !String.IsNullOrEmpty(name) && assembly.GetManifestResourceInfo(makeTemplateName(name)) != null;
        }

        private String makeTemplateName(String name)
        {
            return $"Viewer.BackendTemplates.{name.ToLowerInvariant()}.html";
        }
    }
}