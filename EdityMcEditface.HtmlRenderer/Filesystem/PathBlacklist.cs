using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This will return true if the files are not on the list provided.
    /// </summary>
    public class PathBlacklist : PathPermissionChain
    {
        private PathList pathList;

        public PathBlacklist(PathList pathList, IPathPermissions next = null)
            : base(next)
        {
            this.pathList = pathList;
        }

        public override bool AllowFile(string path)
        {
            return !pathList.OnList(path) && this.Next(path);
        }
    }
}
