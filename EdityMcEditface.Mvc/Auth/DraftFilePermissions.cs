using EdityMcEditface.HtmlRenderer.Filesystem;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.Mvc.Auth
{
    public class DraftFilePermissions : PathPermissionChain
    {
        private HttpContext context;

        public DraftFilePermissions(IHttpContextAccessor contextAccessor, IPathPermissions next = null) 
            : base(next)
        {
            this.context = contextAccessor.HttpContext;
        }

        public override bool AllowFile(string path, string physicalPath)
        {
            //If the file ends with .draft make sure the user has write permission
            var extension = Path.GetExtension(path);
            if(extension == ".draft")
            {
                return context.User.IsInRole(Roles.CreateDrafts);
            }

            //Otherwise return true
            return true;
        }
    }
}
