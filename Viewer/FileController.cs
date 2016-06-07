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
using CommonMarkTools.Renderer;
using CommonMarkTools.Renderer.HtmlRenderers;

namespace Viewer
{
    public class FileController : ApiController
    {
        private static char[] seps = { '|' };
        private static Assembly assembly = Assembly.GetExecutingAssembly();

        public HttpResponseMessage Get(String file)
        {
            try
            {
                bool parse = true;
                var extension = Path.GetExtension(file).ToLowerInvariant();

                using (var templateStream = new DisposeShim<Stream>())
                {
                    switch (extension)
                    {
                        case ".edit":
                            parse = true;
                            templateStream.Target = assembly.GetManifestResourceStream($"Viewer.BackendTemplates.edit.html");
                            break;
                        case ".text":
                            parse = false;
                            break;
                        case ".md":
                        case ".html":
                            throw new FileNotFoundException("Not supporting these file types", file);
                        default:
                            templateStream.Target = File.OpenRead("template.html");
                            break;
                    }

                    //Fix file name
                    if (extension.Length != 0 && file.Length > extension.Length)
                    {
                        file = file.Remove(file.Length - extension.Length);
                    }
                    file = file + ".md";

                    String content;
                    using (var reader = new StreamReader(file))
                    {
                        if (parse)
                        {
                            var identifier = new DefaultHtmlTagIdentiifer();
                            var renderers = new TemplatedHtmlRenderer();
                            renderers.openDoc(templateStream.Target);
                            var tagMap = new HtmlTagMap(renderers.getRenderer);
                            CommonMarkSettings.Default.OutputDelegate = (doc, output, settings) => new FileTemplateHtmlFormatter(tagMap, identifier, output, settings).WriteDocument(doc);

                            using (var writer = new StringWriter())
                            {
                                CommonMarkConverter.Convert(reader, writer);
                                content = writer.ToString();
                            }
                        }
                        else
                        {
                            content = $@"<html><head><title>{file}</title><meta charset=""utf-8"" /></head><body><pre>{reader.ReadToEnd()}</pre></body></html>";
                        }
                    }

                    return htmlResponse(content);
                }
            }
            catch (FileNotFoundException)
            {
                return statusCodeResponse(HttpStatusCode.NotFound);
            }
            catch (Exception)
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
            if (split.Length != 2)
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