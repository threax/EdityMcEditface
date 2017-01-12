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
        bool AllowFile(String path);
    }
}
