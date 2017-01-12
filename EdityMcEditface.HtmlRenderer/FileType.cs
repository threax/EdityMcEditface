using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    /// <summary>
    /// Enum for the type of a file.
    /// </summary>
    [Flags]
    public enum FileType
    {
        /// <summary>
        /// None file type.
        /// </summary>
        None = 0,

        /// <summary>
        /// The file is content and considered part of the website that is produced.
        /// These files can generally be edited by anyone and will be scanned to determine
        /// what the output should be.
        /// </summary>
        Content = 1 << 0,

        /// <summary>
        /// The file is a project file and will only be considered part of the website if
        /// it is referenced from content.
        /// </summary>
        Project = 1 << 1,

        /// <summary>
        /// All file types.
        /// </summary>
        All = Content | Project
    }
}
