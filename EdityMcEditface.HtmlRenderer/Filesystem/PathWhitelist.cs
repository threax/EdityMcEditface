using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This will return true if the files are on the list provided.
    /// </summary>
    public class PathWhitelist : PathPermissionChain
    {
        private PathList pathList;

        public PathWhitelist(PathList pathList, IPathPermissions next = null)
            : base(next)
        {
            this.pathList = pathList;
        }

        public override bool AllowFile(string path, String physicalPath)
        {
            return pathList.OnList(path) && this.Next(path, physicalPath);
        }
    }
}
