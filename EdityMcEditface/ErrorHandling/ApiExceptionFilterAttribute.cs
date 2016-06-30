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
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var validationException = context.Exception as ValidationException;
            if (validationException != null)
            {
                context.Result = new JsonResult(extractErrors(context.ModelState))
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        private static Dictionary<String, String> extractErrors(ModelStateDictionary modelState)
        {
            var errors = new Dictionary<String, String>(modelState.ErrorCount);
            foreach(var item in modelState)
            {
                if(item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(var error in item.Value.Errors)
                    {
                        sb.AppendLine(error.ErrorMessage);
                    }
                    errors[item.Key] = sb.ToString();
                }
            }
            return errors;
        }
    }
}
