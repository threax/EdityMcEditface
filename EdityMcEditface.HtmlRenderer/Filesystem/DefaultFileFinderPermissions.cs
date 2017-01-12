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
            return AllowOutputCopyValue;
        }

        public bool AllowRead(FileFinder fileFinder, string path)
        {
            return AllowReadValue;
        }

        public bool AllowWrite(FileFinder fileFinder, string path)
        {
            return AllowWriteValue;
        }

        public bool TreatAsContent(FileFinder fileFinder, string path)
        {
            return TreatAsContentValue;
        }

        public bool AllowOutputCopyValue { get; set; } = true;

        public bool AllowReadValue { get; set; } = true;

        public bool AllowWriteValue { get; set; } = true;

        public bool TreatAsContentValue { get; set; } = true;
    }
}
