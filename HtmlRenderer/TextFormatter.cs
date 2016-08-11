using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public static class TextFormatter
    {
        public static String formatText(String text, ValueProvider environment, Func<String, String> escapeFunc, char openingDelimiter = '{', char closingDelimiter = '}')
        {
            StringBuilder output = new StringBuilder(text.Length);
            var textStart = 0;
            var bracketStart = 0;
            var bracketEnd = 0;
            var bracketCount = 0;
            var bracketCheck = 0;
            for (var i = 0; i < text.Length; ++i)
            {
                if (text[i] == openingDelimiter)
                {
                    //Count up opening brackets
                    bracketStart = i;
                    bracketCount = 1;
                    while(++i < text.Length && text[i] == openingDelimiter)
                    {
                        ++bracketCount;
                    }

                    //Find closing bracket chain, ignore if mismatched or whitespace
                    bracketCheck = bracketCount;
                    while (++i < text.Length)
                    {
                        if((text[i] == closingDelimiter && --bracketCheck == 0) || Char.IsWhiteSpace(text[i]))
                        {
                            break;
                        }
                    }

                    //If the check got back to 0 we found a variable
                    if(bracketCheck == 0)
                    {
                        bracketEnd = i;
                        var length = bracketEnd - bracketStart - bracketCount - 1;
                        if (length > 0)
                        {
                            var variable = text.Substring(bracketStart + bracketCount, length);
                            String value;
                            if (variable[0] == '|') //Starts with a pipe, pass it to the client side without the pipe.
                            {
                                value = $"{openingDelimiter}{variable.Substring(1)}{closingDelimiter}";
                            }
                            else
                            {
                                value = environment.getValue(variable, "");
                            }
                            output.Append(text.Substring(textStart, bracketStart - textStart));
                            if (environment.shouldEncodeOutput(variable))
                            {
                                value = escapeFunc(value);
                            }
                            output.Append(value);
                        }
                        textStart = i + 1;
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