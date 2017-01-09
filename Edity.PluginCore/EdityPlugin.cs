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
        /// <summary>
        /// This funciton is called at the end of the Startup constructor. It will pass the configuration files for the
        /// site and the server.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        /// <param name="config">The application configuration.</param>
        /// <param name="serverConfig">The edity server configuration.</param>
        public virtual void OnStartup(IHostingEnvironment env, IConfigurationRoot config, IConfigurationRoot serverConfig)
        {

        }

        /// <summary>
        /// This function is called at the beginning of the ConfigureServices function on startup.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public virtual void ConfigureServicesStart(IServiceCollection services)
        {

        }

        /// <summary>
        /// This function is called at the end of the ConfigureServices function on startup.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public virtual void ConfigureServicesEnd(IServiceCollection services)
        {

        }

        /// <summary>
        /// This function is called at the beginning of the Configure function on startup.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public virtual void ConfigureStart(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }

        /// <summary>
        /// This function is called at the end of the Configure function on startup.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public virtual void ConfigureEnd(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

        }
    }
}
