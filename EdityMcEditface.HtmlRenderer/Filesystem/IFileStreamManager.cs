using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This interface opens file streams for the FileFinder.
    /// </summary>
    public interface IFileStreamManager
    {
        /// <summary>
        /// Open a read stream. If you need to customize the stream
        /// to read from, override this function. The default returns the file directly
        /// from the file system.
        /// This function does not chain.
        /// </summary>
        /// <param name="originalFile">The original path to the file.</param>
        /// <param name="normalizedFile">The path to the file normalized by NormalizePath.</param>
        /// <returns></returns>
        Stream OpenReadStream(String originalFile, String normalizedFile);

        /// <summary>
        /// Copy a file from source to dest.
        /// </summary>
        /// <param name="source">The relative source path for the file.</param>
        /// <param name="physicalSource">The full path to the source file.</param>
        /// <param name="physicalDest">The full path to the destination file.</param>
        void CopyFile(String source, String physicalSource, String physicalDest);
    }
}
