using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Services
{
    public class PathBaseInjector : IPathBaseInjector
    {
        private IHttpContextAccessor httpContextAccessor;

        public PathBaseInjector(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public String PathBase
        {
            get
            {
                return this.httpContextAccessor.HttpContext.Request.PathBase;
            }
        }
    }
}
