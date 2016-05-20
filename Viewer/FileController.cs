using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace OwinSelfhostSample
{
    public class FileController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        public HttpResponseMessage Get(String file)
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
$@"
<!DOCTYPE html>
<html>
<head>
<title>{file} as html</title>
</head>
    <body>
        {content}
    </body>
</html>
";

            var response = new HttpResponseMessage();
            response.Content = new StringContent(page);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}