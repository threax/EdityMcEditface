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
        /// <param name="path">The relative path to access inside the edity folders.</param>
        /// <param name="physicalPath">The path fully expanded on the file system.</param>
        /// <returns>True to allow, false to deny.</returns>
        bool AllowFile(String path, String physicalPath);
    }
}
