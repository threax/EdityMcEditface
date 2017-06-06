using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface SiteBuilder
    {
        /// <summary>
        /// Add a task that runs before build.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void addPreBuildTask(BuildTask task);

        /// <summary>
        /// Add a task that runs after the build.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void addPostBuildTask(BuildTask task);

        /// <summary>
        /// Build the site.
        /// </summary>
        void BuildSite();

        /// <summary>
        /// Open a write stream to file in the destination folder.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <returns>A stream to the file.</returns>
        Stream OpenOutputWriteStream(String file);

        /// <summary>
        /// Determine if a given file exists in the site output.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>True if the output contains the file, false otherwise.</returns>
        bool DoesOutputFileExist(String file);
    }
}
