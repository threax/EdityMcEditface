using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// Base class to build a chain of permission objects.
    /// </summary>
    public abstract class PathPermissionChain : IPathPermissions
    {
        private IPathPermissions next;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="next">The next permission in the chain.</param>
        public PathPermissionChain(IPathPermissions next)
        {
            this.next = next;
        }

        public abstract bool AllowFile(string path);

        /// <summary>
        /// Call the next item in the chain. Can optionally pass a different value to return
        /// if next is null. Defaults to true.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <param name="nullValue">The value to return if next is null. Defaults to true.</param>
        /// <returns>The value of next's AllowFile or nullValue if next is null.</returns>
        public bool Next(string path, bool nullValue = true)
        {
            if (next != null)
            {
                return next.AllowFile(path);
            }
            else
            {
                return nullValue;
            }
        }
    }
}
