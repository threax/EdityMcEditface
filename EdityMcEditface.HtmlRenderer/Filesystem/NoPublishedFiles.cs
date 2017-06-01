using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class NoPublishedFiles : IPublishedFileManager
    {
        public bool IsPublishableFile(string normalizedFile)
        {
            return false;
        }

        public bool SendPageToDraft(string normalizedPath)
        {
            return false;
        }
    }
}
