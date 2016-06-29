using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    /// <summary>
    /// Get values for the TextFormatter function.
    /// </summary>
    public interface ValueProvider
    {
        /// <summary>
        /// Get a particluar value given a key. If the key does not exist use the default value provided.
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <param name="defaultVal">The default value</param>
        /// <returns>The value from this provider or the default if it does not exist.</returns>
        string getValue(String key, String defaultVal);

        /// <summary>
        /// This function should return true if a particular value should be encoded. You will probably
        /// almost always want to return true from this function so that things are properly escaped
        /// especially for any values provided by the user.
        /// </summary>
        /// <param name="variable">The variable to test for escaping.</param>
        /// <returns>True to escape the variable, false if it is already encoded.</returns>
        bool shouldEncodeOutput(string variable);
    }
}
