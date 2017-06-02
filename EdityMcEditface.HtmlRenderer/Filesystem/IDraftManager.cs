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
        /// <param name="file">The relative name of the file.</param>
        /// <param name="physicalFile">The full normalized path of the file.</param>
        /// <param name="fileFinder">The file finder to use to write. Using this will check permissions correctly.</param>
        /// <returns></returns>
        bool SendPageToDraft(String file, String physicalFile, IFileFinder fileFinder);

        /// <summary>
        /// Get all items that can be drafted no matter if they are currently drafted or not.
        /// </summary>
        /// <returns>A list of all pages.</returns>
        IEnumerable<String> GetAllDraftables(IFileFinder fileFinder);

        /// <summary>
        /// Get all draftable items that have a draft.
        /// </summary>
        /// <returns>All pages with a draft.</returns>
        IEnumerable<String> GetDrafts(IFileFinder fileFinder);
    }
}
