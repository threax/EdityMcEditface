using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edity.McEditface
{
    public static class ControllerExtensions
    {

        public static HttpResponseMessage statusCodeResponse(this ApiController apiController, HttpStatusCode code)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = code;
            return response;
        }

        public static HttpResponseMessage htmlResponse(this ApiController apiController, String content, String mimeType = "text/html")
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return response;
        }

        public static HttpResponseMessage streamResponse(this ApiController apiController, Stream content, String mimeType)
        {
            StreamContent streamContent = new StreamContent(content);
            var task = streamContent.LoadIntoBufferAsync();
            task.Wait();

            var response = new HttpResponseMessage();
            response.Content = streamContent;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return response;
        }

        public static String escapeTemplate(this ApiController apiController, String name)
        {
            //this could be better
            var dir = Path.GetDirectoryName(name);
            var file = Path.GetFileName(name);
            return dir.Replace('\\', '.').Replace('/', '.').Replace('-', '_') + "." + file;
        }
    }
}
