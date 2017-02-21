using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface SiteBuilder
    {
        void addPreBuildTask(BuildTask task);

        void addPostBuildTask(BuildTask task);

        void BuildSite();

        /// <summary>
        /// Open a write stream to file in the destination folder.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <returns>A stream to the file.</returns>
        Stream OpenOutputWriteStream(String file);
    }
}
