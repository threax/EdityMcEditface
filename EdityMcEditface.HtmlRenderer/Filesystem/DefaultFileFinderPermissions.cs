using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class DefaultFileFinderPermissions : IFileFinderPermissions
    {
        public bool AllowOutputCopy(FileFinder fileFinder, string path)
        {
            return OutputCopyPermission.Allow(path);
        }

        public bool AllowRead(FileFinder fileFinder, string path)
        {
            return ReadPermission.Allow(path);
        }

        public bool AllowWrite(FileFinder fileFinder, string path)
        {
            return WritePermission.Allow(path);
        }

        public bool TreatAsContent(FileFinder fileFinder, string path)
        {
            return TreatAsContentPermission.Allow(path);
        }

        public PermissionClass OutputCopyPermission { get; set; } = new PermissionClass();

        public PermissionClass ReadPermission { get; set; } = new PermissionClass();

        public PermissionClass WritePermission { get; set; } = new PermissionClass();

        public PermissionClass TreatAsContentPermission { get; set; } = new PermissionClass();
    }
}
