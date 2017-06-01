using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// The permissions allows the user to modify how a FileFinder interacts with files.
    /// </summary>
    public interface IFileFinderPermissions
    {
        /// <summary>
        /// True if this file finder is allowed to write to a path.
        /// </summary>
        /// <param name="fileFinder">The FileFinder that triggered the check.</param>
        /// <param name="path">The path the FileFinder is trying to write.</param>
        /// <param name="physicalPath">The full path on disk to the file.</param>
        /// <returns>True if the file can be written.</returns>
        bool AllowWrite(FileFinder fileFinder, string path, string physicalPath);

        /// <summary>
        /// True if this file finder is allowed to read from a path.
        /// </summary>
        /// <param name="fileFinder">The FileFinder that triggered the check.</param>
        /// <param name="path">The path the FileFinder is trying to read.</param>
        /// <param name="physicalPath">The full path on disk to the file.</param>
        /// <returns>True if the file can be read.</returns>
        bool AllowRead(FileFinder fileFinder, string path, string physicalPath);

        /// <summary>
        /// True if this file finder is allowed to copy path as output.
        /// This is a rare thing to disable.
        /// </summary>
        /// <param name="fileFinder">The FileFinder that triggered the check.</param>
        /// <param name="path">The path the FileFinder is trying to load.</param>
        /// <param name="physicalPath">The full path on disk to the file.</param>
        /// <returns>True if the file can be written to output.</returns>
        bool AllowOutputCopy(FileFinder fileFinder, string path, string physicalPath);

        /// <summary>
        /// True if this file finder should treat path as content.
        /// </summary>
        /// <param name="fileFinder">The FileFinder that triggered the check.</param>
        /// <param name="path">The path the FileFinder is trying to load.</param>
        /// <param name="physicalPath">The full path on disk to the file.</param>
        /// <returns>True if the file should be considered content.</returns>
        bool TreatAsContent(FileFinder fileFinder, string path, string physicalPath);
    }
}
