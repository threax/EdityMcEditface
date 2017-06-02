using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class FileStreamManager : IFileStreamManager
    {
        /// <summary>
        /// Open a read stream. If you need to customize the stream
        /// to read from, override this function. The default returns the file directly
        /// from the file system.
        /// This function does not chain.
        /// </summary>
        /// <param name="file">The original requested path to the file.</param>
        /// <param name="physicalFile">The path to the file on the physical drive.</param>
        /// <returns></returns>
        public virtual Stream OpenReadStream(String file, String physicalFile)
        {
            return File.Open(physicalFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Copy a file from source to dest.
        /// </summary>
        /// <param name="physicalSource"></param>
        /// <param name="physicalDest"></param>
        public virtual void CopyFile(String source, String physicalSource, String physicalDest)
        {
            File.Copy(physicalSource, physicalDest);
        }
    }
}
