using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.FileInfo
{
    public interface ITargetFileInfoProvider
    {
        ITargetFileInfo GetFileInfo(String file, String pathBase);
    }
}
