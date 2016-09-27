using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExceptionErrorHandler
{
    /// <summary>
    /// This class is an error result with exception details.
    /// </summary>
    public class ExceptionErrorResult : ErrorResult
    {
        public ExceptionErrorResult(Exception ex)
            :base(ex.Message)
        {
            this.Exception = ex;
        }

        public Exception Exception { get; set; }
    }
}
