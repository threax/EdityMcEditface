using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Services;
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
        String pathBase;

        public CompileRequestDetector(IHttpContextAccessor accessor, IPathBaseInjector pathInjector)
        {
            this.context = accessor.HttpContext;
            this.pathBase = pathInjector.PathBase;
        }

        public bool IsCompileRequest
        {
            get
            {
                //This is pretty lame, but is always used through this interface, so it can be improved
                var path = "edity/Publish/Compile";
                if (!String.IsNullOrEmpty(this.pathBase))
                {
                    path = pathBase.TrimStartingPathChars().EnsureTrailingPathSlash() + path;
                }
                var reqPath = context.Request.Path.ToString().TrimStartingPathChars().TrimTrailingPathChars();
                return reqPath.Equals(path, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
