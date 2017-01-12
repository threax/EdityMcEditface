using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public interface IFileFinderPermissions
    {
        bool AllowWrite(FileFinder fileFinder, string file);
        bool AllowRead(FileFinder fileFinder, string layoutName);
        bool AllowOutputCopy(FileFinder fileFinder);
        bool TreatAsContent(FileFinder fileFinder);
    }
}
