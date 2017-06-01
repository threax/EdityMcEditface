using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class NoDrafts : IDraftManager
    {
        public bool IsDraftedFile(string normalizedFile)
        {
            return false;
        }

        public bool SendPageToDraft(string normalizedPath)
        {
            return false;
        }
    }
}
