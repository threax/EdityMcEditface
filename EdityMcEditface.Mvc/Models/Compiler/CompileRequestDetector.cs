using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Compiler
{
    public class CompileRequestDetector : ICompileRequestDetector
    {
        HttpContext context;

        public CompileRequestDetector(IHttpContextAccessor accessor)
        {
            this.context = accessor.HttpContext;
        }

        public bool IsCompileRequest
        {
            get
            {
                //This is pretty lame, but is always used through this interface, so it can be improved
                return context.Request.Path.Equals(new PathString("/edity/Compile/Compile"), StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
