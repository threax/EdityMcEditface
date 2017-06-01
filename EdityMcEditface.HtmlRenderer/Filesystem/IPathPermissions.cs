using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This interface helps determine if a file has permission to be accessed.
    /// </summary>
    public interface IPathPermissions
    {
        /// <summary>
        /// Determine if path should be allowed to be accessed.
        /// </summary>
        /// <param name="path">The path to access.</param>
        /// <returns>True to allow, false to deny.</returns>
        bool AllowFile(String path);
    }
}
