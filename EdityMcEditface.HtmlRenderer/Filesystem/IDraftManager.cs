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
        /// Get the draft status of a file. If you cannot determine the file's status, return null.
        /// </summary>
        /// <param name="file">The file to lookup draft status for.</param>
        /// <param name="physicalFile">The full path to the file on the disk.</param>
        /// <param name="fileFinder">The file finder to lookup files with.</param>
        /// <returns>The draft info for the file or null if no status can be computed.</returns>
        DraftInfo GetDraftStatus(String file, String physicalFile, IFileFinder fileFinder);

        /// <summary>
        /// This function is called when a page is erased, the draft info should be cleaned up at this time.
        /// </summary>
        /// <param name="file">The file for the page to erase.</param>
        /// <param name="physicalPath">The path on the operating system for the file.</param>
        void PageErased(string file, string physicalPath);
    }
}
