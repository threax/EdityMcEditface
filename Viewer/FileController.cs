using System;
using System.Collections.Generic;
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
        public string Get(String file)
        {
            return file;
        }
    }
}