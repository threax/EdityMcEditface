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

namespace Viewer
{
    public class EmbeddedFileController : ApiController
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static CommonMarkContentTypeProvider contentTypeProvider = new CommonMarkContentTypeProvider();

        public HttpResponseMessage Get(String file)
        {
            try
            {
                var mime = contentTypeProvider.Mappings[Path.GetExtension(file).ToLowerInvariant()];
                using (var stream = loadResource(file))
                {
                    if(stream == null)
                    {
                        throw new FileNotFoundException();
                    }
                    return streamResponse(stream, mime);
                }
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

        public HttpResponseMessage streamResponse(Stream content, String mimeType)
        {
            StreamContent streamContent = new StreamContent(content);
            var task = streamContent.LoadIntoBufferAsync();
            task.Wait();

            var response = new HttpResponseMessage();
            response.Content = streamContent;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return response;
        }

        private Stream loadResource(String name)
        {
            return assembly.GetManifestResourceStream(makeTemplateName(name));
        }

        private String makeTemplateName(String name)
        {
            return $"Viewer.Resources.{escapeTemplate(name)}";
        }

        private String escapeTemplate(String name)
        {
            //this could be better
            var dir = Path.GetDirectoryName(name);
            var file = Path.GetFileName(name);
            return dir.Replace('\\', '.').Replace('/', '.').Replace('-', '_') + "." + file;
        }
    }
}