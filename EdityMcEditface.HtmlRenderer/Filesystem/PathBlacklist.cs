using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This will return true if the files are not on the list provided.
    /// </summary>
    public class PathBlacklist : IPathPermissions
    {
        private PathList pathList;

        public PathBlacklist(PathList pathList)
        {
            this.pathList = pathList;
        }

        public bool AllowFile(string path)
        {
            return !pathList.OnList(path);
        }
    }
}
