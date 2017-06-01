using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class DefaultFileFinderPermissions : IFileFinderPermissions
    {
        public bool AllowOutputCopy(FileFinder fileFinder, string path, string physicalPath)
        {
            return OutputCopyPermission.Allow(path, physicalPath);
        }

        public bool AllowRead(FileFinder fileFinder, string path, string physicalPath)
        {
            return ReadPermission.Allow(path, physicalPath);
        }

        public bool AllowWrite(FileFinder fileFinder, string path, string physicalPath)
        {
            return WritePermission.Allow(path, physicalPath);
        }

        public bool TreatAsContent(FileFinder fileFinder, string path, string physicalPath)
        {
            return TreatAsContentPermission.Allow(path, physicalPath);
        }

        public PermissionClass OutputCopyPermission { get; set; } = new PermissionClass();

        public PermissionClass ReadPermission { get; set; } = new PermissionClass();

        public PermissionClass WritePermission { get; set; } = new PermissionClass();

        public PermissionClass TreatAsContentPermission { get; set; } = new PermissionClass();
    }
}
