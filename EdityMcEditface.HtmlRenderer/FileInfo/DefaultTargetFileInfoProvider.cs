using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.FileInfo
{
    public class DefaultTargetFileInfoProvider : ITargetFileInfoProvider
    {
        private String defaultFile;

        public DefaultTargetFileInfoProvider(String defaultFile)
        {
            this.defaultFile = defaultFile;
        }

        public ITargetFileInfo GetFileInfo(string file, string pathBase)
        {
            return new DefaultTargetFileInfo(file, pathBase, defaultFile);
        }
    }
}
