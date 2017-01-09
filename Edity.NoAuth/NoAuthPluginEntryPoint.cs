using Edity.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: Edity.NoAuth.NoAuthPluginEntryPoint()]

namespace Edity.NoAuth
{
    public class NoAuthPluginEntryPoint : EdityPluginEntryPointAttribute
    {
        public override IEnumerable<EdityPlugin> CreatePlugins()
        {
            yield return new NoAuthPlugin();
        }
    }
}
