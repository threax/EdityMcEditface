using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This class manages published files, determining which ones are publishable
    /// and getting / setting the current publish status.
    /// </summary>
    public interface IPublishedFileManager
    {
        /// <summary>
        /// Determine if the path specified by the normalized file name is correct.
        /// </summary>
        /// <param name="normalizedFile"></param>
        /// <returns></returns>
        bool IsPublishableFile(String normalizedFile);

        /// <summary>
        /// Send a page to draft. Will return true if the page is updated, false if 
        /// no changes are made (page not found, other errors).
        /// </summary>
        /// <param name="file">The incoming path to the file.</param>
        /// <param name="normalizedPath">The full normalized path of the file.</param>
        /// <returns></returns>
        bool SendPageToDraft(String file, String normalizedPath);
    }
}
