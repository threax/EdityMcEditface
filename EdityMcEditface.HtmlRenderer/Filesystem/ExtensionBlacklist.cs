using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This class will black list extensions from being written if they are on the list.
    /// </summary>
    public class ExtensionBlacklist : PathPermissionChain
    {
        private List<String> extensions;

        public ExtensionBlacklist(IEnumerable<String> extensions, IPathPermissions next = null)
            :base(next)
        {
            this.extensions = new List<string>(extensions);
        }

        public override bool AllowFile(string path)
        {
            var extension = Path.GetExtension(path);
            return !extensions.Contains(extension) && this.Next(path);
        }
    }
}
