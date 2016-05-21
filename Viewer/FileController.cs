using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Viewer.TemplateFormatter;

namespace OwinSelfhostSample
{
    public class FileController : ApiController
    {
        // GET api/values 
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5 
        public HttpResponseMessage Get(String file)
        {
            if (!Path.GetExtension(file).Equals(".md", StringComparison.InvariantCultureIgnoreCase))
            {
                return statusCodeResponse(HttpStatusCode.NotFound);
            }

            //var identifier = new HtmlTagIdentifier();
            //var rendererFinder
            //var tagMap = new HtmlTagMap()
            //CommonMarkSettings.Default.OutputDelegate = (doc, output, settings) => new FileTemplateHtmlFormatter(".", output, settings).WriteDocument(doc);

            

            try
            {
                String content;
                using (var reader = new StreamReader(file))
                {
                    using (var writer = new StringWriter())
                    {
                        CommonMarkConverter.Convert(reader, writer);
                        content = writer.ToString();
                    }
                }

                var page =
$@"<!DOCTYPE html>
<html>
<head>
<title>{file} as html</title>
</head>
    <body>
        {content}
    </body>
</html>
";
                return htmlResponse(page);
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
    }
}