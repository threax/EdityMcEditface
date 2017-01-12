using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class PermissionClass
    {
        public bool Permit { get; set; } = true;

        public IPathPermissions Permissions { get; set; }

        public bool Allow(String path)
        {
            return Permit && (Permissions == null || Permissions.AllowFile(path));
        }
    }
}
