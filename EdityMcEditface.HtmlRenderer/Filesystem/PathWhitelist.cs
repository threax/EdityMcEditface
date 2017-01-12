using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This will return true if the files are on the list provided.
    /// </summary>
    public class PathWhitelist : IPathPermissions
    {
        private PathList pathList;

        public PathWhitelist(PathList pathList)
        {
            this.pathList = pathList;
        }

        public bool AllowFile(string path)
        {
            return pathList.OnList(path);
        }
    }
}
