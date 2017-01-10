using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public static class StringExtensions
    {
        /// <summary>
        /// Replace all sequences of whitespace characters (defined by char.iswhitespace) with a single space.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String SingleSpaceWhitespace(this String s)
        {
            if(s.Length == 0)
            {
                return s;
            }

            StringBuilder sb = new StringBuilder(s.Length);
            int textStart = 0;
            int length = s.Length;
            bool inWhitespace = char.IsWhiteSpace(s[0]);
            for(int i = 0; i < length; ++i)
            {
                var c = s[i];
                if (inWhitespace)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        sb.Append(" ");
                        textStart = i;
                        inWhitespace = false;
                    }
                }
                else
                {
                    if (char.IsWhiteSpace(c))
                    {
                        sb.Append(s.Substring(textStart, i - textStart));
                        inWhitespace = true;
                    }
                }
            }
            return sb.ToString();
        }

        public static String JsonEscape(this String s)
        {
            s = JsonConvert.ToString(s);
            if (s.Length > 2)
            {
                s = s.Substring(1, s.Length - 2);
            }
            return s;
        }
    }
}
