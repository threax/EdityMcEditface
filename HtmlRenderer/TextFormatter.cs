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
            int textStart = 0;
            int bracketStart = 0;
            int bracketEnd = 0;
            int bracketCount = 0;
            int bracketCheck = 0;
            String variable;
            String bracketVariable;
            String value;
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
                        //Output everything up to this point
                        output.Append(text.Substring(textStart, bracketStart - textStart));
                        bracketEnd = i;
                        bracketVariable = text.Substring(bracketStart, bracketEnd - bracketStart + 1);

                        switch (bracketCount)
                        {
                            case 1:
                                //1 bracket, output as is
                                output.Append(bracketVariable);
                                break;
                            case 2:
                                variable = bracketVariable.Substring(bracketCount, bracketVariable.Length - bracketCount * 2);
                                value = environment.getValue(variable, bracketVariable);
                                if (environment.shouldEncodeOutput(variable))
                                {
                                    value = escapeFunc(value);
                                }
                                output.Append(value);
                                break;
                            default:
                                //Multiple brackets, escape by removing one
                                output.Append(bracketVariable.Substring(1, bracketVariable.Length - 2));
                                break;
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