using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edity.PluginCore
{
    /// <summary>
    /// This attribute should be placed on a plugin assembly to load the EdityPlugins when
    /// CreatePlugins is called.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class EdityPluginEntryPointAttribute : Attribute
    {
        /// <summary>
        /// This funciton is called on startup to load any EdityPlugin interfaces in the client plugin.
        /// </summary>
        /// <returns>An enumerable over all the EdityPlugins this plugin creates.</returns>
        public abstract IEnumerable<EdityPlugin> CreatePlugins();
    }
}
