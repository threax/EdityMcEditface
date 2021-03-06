﻿using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Trim leading / and \ from a string.
        /// </summary>
        /// <param name="input">The path to trim.</param>
        /// <returns>The path without any leading / or \.</returns>
        public static String TrimTrailingPathChars(this String input)
        {
            return input.TrimEnd(PathTrimChars);
        }

        /// <summary>
        /// Ensure that the path starts with a / or \. If a character needs to be added it will add separatorChar.
        /// </summary>
        /// <param name="input">The string to modify.</param>
        /// <param name="separatorChar">The separator char to add.</param>
        /// <returns></returns>
        public static String EnsureStartingPathSlash(this String input, char separatorChar = '/')
        {
            if (input.Length == 0 || (input[0] != '\\' && input[0] != '/'))
            {
                return separatorChar + input;
            }
            return input;
        }

        /// <summary>
        /// Ensure that the path starts with a / or \. If a character needs to be added it will add separatorChar.
        /// </summary>
        /// <param name="input">The string to modify.</param>
        /// <param name="separatorChar">The separator char to add.</param>
        /// <returns></returns>
        public static String EnsureTrailingPathSlash(this String input, char separatorChar = '/')
        {
            if (input.Length == 0 || (input[input.Length - 1] != '\\' && input[input.Length - 1] != '/'))
            {
                return input + separatorChar;
            }
            return input;
        }
    }
}
