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
using CommonMarkTools.HtmlRenderer;

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
                var markdownFile = file + ".html";

                TemplateEnvironment environment = new TemplateEnvironment("/" + file);
                using (var markdown = new StreamReader(File.OpenRead(markdownFile)))
                {
                    switch (extension)
                    {
                        case ".edit":
                        case ".html":
                            if (File.Exists("layouts/edit.html"))
                            {
                                using (var template = new StreamReader(File.OpenRead("layouts/edit.html")))
                                {
                                    return parsedResponse(markdown, template, environment);
                                }
                            }
                            else
                            {
                                using (var template = new StreamReader(assembly.GetManifestResourceStream("Viewer.BackendTemplates.edit.html")))
                                {
                                    return parsedResponse(markdown, template, environment);
                                }
                            }
                        case ".text":
                            return viewMarkdownResponse(markdown);
                        default:
                            using (var template = new StreamReader(File.OpenRead("layouts/default.html")))
                            {
                                return parsedResponse(markdown, template, environment);
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

        public HttpResponseMessage parsedResponse(StreamReader markdown, StreamReader template, TemplateEnvironment environment)
        {
            DocumentRenderer dr = new DocumentRenderer(template.ReadToEnd(), environment);
            return htmlResponse(dr.getDocument(markdown.ReadToEnd()));
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