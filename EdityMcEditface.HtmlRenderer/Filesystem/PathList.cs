using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// A path list holds files and directories that can be checked for matches.
    /// </summary>
    public class PathList
    {
        private List<String> files = new List<string>();
        private List<String> dirs = new List<string>();

        /// <summary>
        /// Add a file to the list. Files must match the input paths in order to count as a match.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public void AddFile(String path)
        {
            this.files.Add(Path.GetFullPath(path.EnsureStartingPathSlash()));
        }

        /// <summary>
        /// Add a directory to the list. Any files in a directory and the directory itself will be considered part of the list.
        /// </summary>
        /// <param name="path"></param>
        public void AddDirectory(String path)
        {
            this.dirs.Add(Path.GetFullPath(path.EnsureStartingPathSlash()));
        }

        /// <summary>
        /// Determine if a path is on the list either as a file match or nested under a listed directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool OnList(String path)
        {
            var normalized = Path.GetFullPath(path.EnsureStartingPathSlash());
            foreach(var dir in dirs)
            {
                if(normalized.StartsWith(dir, StringComparison.OrdinalIgnoreCase))
                {
                    if(normalized.Length == dir.Length)
                    {
                        return true;
                    }

                    //Make sure the next character in the input string is a separator, or else this is not correct.
                    var testChar = normalized[dir.Length];
                    return testChar == '\\' || testChar == '/';
                }
            }

            //No dir match, check files
            return files.Any(i => normalized.Equals(i, StringComparison.OrdinalIgnoreCase));
        }
    }
}
