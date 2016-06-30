using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.ErrorHandling
{
    /// <summary>
    /// This filter checks for the exceptions thrown by the attributes defined in this system
    /// and converts them to the appropriate json result.
    /// </summary>
    public class ExceptionToJsonFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var validationException = context.Exception as ValidationException;
            if (validationException != null)
            {
                context.Result = new JsonResult(new ErrorResult(context.ModelState, validationException.Message))
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
