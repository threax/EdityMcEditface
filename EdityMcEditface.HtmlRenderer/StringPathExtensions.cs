using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public static class StringPathExtensions
    {
        private static char[] PathTrimChars = { '\\', '/' };
        /// <summary>
        /// Trim leading / and \ from a string.
        /// </summary>
        /// <param name="input">The path to trim.</param>
        /// <returns>The path without any leading / or \.</returns>
        public static String TrimStartingPathChars(this String input)
        {
            return input.TrimStart(PathTrimChars);
        }
    }
}
