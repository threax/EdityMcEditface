using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This class will black list extensions from being written unless they are on the list.
    /// Be careful when creating this to specify the empty extension "" unless you want to block 
    /// all files and folders without extensions.
    /// </summary>
    public class ExtensionWhitelist : PathPermissionChain
    {
        private List<String> extensions;

        public ExtensionWhitelist(IEnumerable<String> extensions, IPathPermissions next = null)
             : base(next)
        {
            this.extensions = new List<string>(extensions);
        }

        public override bool AllowFile(string path, String physicalPath)
        {
            var extension = Path.GetExtension(path);
            return extensions.Contains(extension) && this.Next(path, physicalPath);
        }
    }
}
