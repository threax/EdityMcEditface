using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edity.PluginCore
{
    /// <summary>
    /// A concrete plugin implementation. All functions do nothing by default.
    /// </summary>
    public class EdityPlugin : IEdityPlugin
    {
        public virtual void OnStartup(IHostingEnvironment env, IConfigurationRoot config, IConfigurationRoot serverConfig)
        {

        }

        public virtual void ConfigureServicesStart(IServiceCollection services)
        {

        }

        public virtual void ConfigureMvc(IMvcBuilder builder)
        {
            
        }

        public virtual void ConfigureServicesEnd(IServiceCollection services)
        {

        }

        public virtual void ConfigureStart(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }

        public virtual void ConfigureEnd(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }
}
