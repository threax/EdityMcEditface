using CommonMark;
using System;
using System.Collections.Generic;
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
            var content = CommonMarkConverter.Convert("**Hello world! From Commonmark**");

            var response = new HttpResponseMessage();
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}