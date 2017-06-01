using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This class manages drafted files, determining which ones are drafted
    /// and getting / setting the current draft status.
    /// </summary>
    public interface IDraftManager
    {
        /// <summary>
        /// Determine if the path specified by the normalized file name is correct.
        /// </summary>
        /// <param name="normalizedFile"></param>
        /// <returns></returns>
        bool IsDraftedFile(String normalizedFile);

        /// <summary>
        /// Send a page to draft. Will return true if the page is updated, false if 
        /// no changes are made (page not found, other errors).
        /// </summary>
        /// <param name="normalizedPath">The full normalized path of the file.</param>
        /// <returns></returns>
        bool SendPageToDraft(String normalizedPath);
    }
}
