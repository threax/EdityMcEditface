using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Page
{
    /// <summary>
    /// This class can detect if a given file path is publishable.
    /// </summary>
    public interface IPublishedFileDetector
    {
        /// <summary>
        /// Determine if the path specified by the original file name and the normalized
        /// file name is publishable or not.
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="normalizedFile"></param>
        /// <returns></returns>
        bool IsPublishableFile(String originalFile, String normalizedFile);
    }
}
