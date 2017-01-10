using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    /// <summary>
    /// A value provider backed by a dictionary. For convienence the dictionary is a public property
    /// that can be used directly.
    /// </summary>
    public class DictionaryValueProvider : ValueProvider
    {
        /// <summary>
        /// The dictionary of values to use.
        /// </summary>
        public Dictionary<String, String> Values { get; set; } = new Dictionary<string, string>();

        public string getValue(string key, string defaultVal)
        {
            String result;
            if(!Values.TryGetValue(key, out result))
            {
                result = defaultVal;
            }
            return result;
        }

        public bool shouldEncodeOutput(string variable)
        {
            return true;
        }
    }
}
