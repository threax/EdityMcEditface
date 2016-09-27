﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
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
        private bool detailedInternalServerError;

        /// <summary>
        /// Constructor. Takes a bool to show detailed Internal Server Error (500) exceptions or not.
        /// If these are detailed the exception will be serialized to json, if not just 500 is returned.
        /// You should only use detailed exceptions for development otherwise you could leak internal
        /// implementation details.
        /// </summary>
        /// <param name="detailedInternalServerError"></param>
        public ExceptionToJsonFilterAttribute(bool detailedInternalServerError)
        {
            this.detailedInternalServerError = detailedInternalServerError;
        }

        public override void OnException(ExceptionContext context)
        {
            //Validation exception becomes a Bad Request (400) and gets a ModelState simplified and serialized to json.
            var validationException = context.Exception as ValidationException;
            if (validationException != null)
            {
                context.Result = new JsonResult(new ModelStateErrorResult(context.ModelState, validationException.Message))
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };

                return;
            }

            //ExceptionErrorResult becomes a ErrorResult with the given status code
            var exceptionErrorResult = context.Exception as ErrorResultException;
            if(exceptionErrorResult != null)
            {
                context.Result = new JsonResult(new ErrorResult(exceptionErrorResult.Message))
                {
                    StatusCode = (int)exceptionErrorResult.StatusCode
                };

                return;
            }

            //File not found becomes a Not Found (404).
            var fileNotFoundException = context.Exception as FileNotFoundException;
            if(fileNotFoundException != null)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);

                return;
            }

            //Other exception types become Internal Server Error (500) and are detailed or not depending on settings.
            if (detailedInternalServerError)
            {
                context.Result = new JsonResult(new ExceptionErrorResult(context.Exception))
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            else
            {
                context.Result = new JsonResult(new ErrorResult("Internal Server Error"))
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
