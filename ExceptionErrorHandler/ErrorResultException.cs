using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EdityMcEditface.ErrorHandling
{
    public class ErrorResultException : Exception
    {
        public ErrorResultException(String message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            :base(message)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}
