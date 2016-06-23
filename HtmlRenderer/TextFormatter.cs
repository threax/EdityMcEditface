using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public static class TextFormatter
    {
        public static String formatText(String text, TemplateEnvironment environment, char openingDelimiter = '{', char closingDelimiter = '}')
        {
            StringBuilder output = new StringBuilder(text.Length);
            var textStart = 0;
            var bracketStart = 0;
            var bracketEnd = 0;
            for (var i = 0; i < text.Length; ++i)
            {
                if(text[i] == openingDelimiter)
                {
                    if (text[i + 1] != openingDelimiter)
                    {
                        bracketStart = i;
                    }
                }
                else if(text[i] == closingDelimiter)
                {
                    if (i + 1 == text.Length || text[i + 1] != '}')
                    {
                        bracketEnd = i;

                        if (bracketStart < bracketEnd - 1)
                        {
                            var variable = text.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
                            String value;
                            if (variable[0] == '|') //Starts with a pipe, pass it to the client side without the pipe.
                            {
                                value = $"{openingDelimiter}{variable.Substring(1)}{closingDelimiter}";
                            }
                            else
                            {
                                value = environment.getVariable(variable, "");
                            }
                            output.Append(text.Substring(textStart, bracketStart - textStart));
                            if (environment.encodeOutput(variable))
                            {
                                value = System.Net.WebUtility.HtmlEncode(value);
                            }
                            output.Append(value);
                            textStart = i + 1;
                        }
                    }
                }
            }

            if (textStart < text.Length)
            {
                output.Append(text.Substring(textStart, text.Length - textStart));
            }
            return output.ToString();
        }
    }
}
