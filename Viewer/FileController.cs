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

        private String currentFile;

        public HttpResponseMessage Get(String file)
        {
            try
            {
                this.currentFile = file;
                var extension = Path.GetExtension(file).ToLowerInvariant();

                //Fix file name
                if (extension.Length != 0 && file.Length > extension.Length)
                {
                    file = file.Remove(file.Length - extension.Length);
                }
                var markdownFile = file + ".md";

                using (var markdown = new StreamReader(File.OpenRead(markdownFile)))
                {
                    switch (extension)
                    {
                        case ".edit":
                            if (File.Exists("edit.html"))
                            {
                                using (var template = File.OpenRead("edit.html"))
                                {
                                    return parsedResponse(markdown, template);
                                }
                            }
                            else
                            {
                                using (var template = assembly.GetManifestResourceStream("Viewer.BackendTemplates.edit.html"))
                                {
                                    return parsedResponse(markdown, template);
                                }
                            }
                        case ".text":
                            return viewMarkdownResponse(markdown);
                        case ".md":
                        case ".html":
                            throw new FileNotFoundException("Not supporting these file types", file);
                        default:
                            using (var template = File.OpenRead("template.html"))
                            {
                                return parsedResponse(markdown, template);
                            }
                    }
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

        public HttpResponseMessage parsedResponse(StreamReader markdown, Stream template)
        {
            var identifier = new DefaultHtmlTagIdentiifer();
            var renderers = new TemplatedHtmlRenderer();
            renderers.openDoc(template);
            var tagMap = new HtmlTagMap(renderers.getRenderer);
            CommonMarkSettings.Default.OutputDelegate = (doc, output, settings) => new FileTemplateHtmlFormatter(tagMap, identifier, output, settings).WriteDocument(doc);

            using (var writer = new StringWriter())
            {
                CommonMarkConverter.Convert(markdown, writer);
                return htmlResponse(writer.ToString());
            }
        }

        public HttpResponseMessage viewMarkdownResponse(StreamReader markdown)
        {
            return htmlResponse($@"<html><head><title>{this.currentFile}</title><meta charset=""utf-8"" /></head><body><pre>{markdown.ReadToEnd()}</pre></body></html>");
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
    }
}