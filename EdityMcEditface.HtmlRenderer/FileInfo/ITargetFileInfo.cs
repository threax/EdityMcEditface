using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.FileInfo
{
    public interface ITargetFileInfo
    {
        string DerivedFileName { get; }
        string Extension { get; }
        string FileNoExtension { get; }
        string HtmlFile { get; }
        bool IsProjectFile { get; }
        string NoHtmlRedirect { get; }
        string OriginalFileName { get; }
        bool PathCanCreateFile { get; }
        bool PointsToHtmlFile { get; }
    }
}
