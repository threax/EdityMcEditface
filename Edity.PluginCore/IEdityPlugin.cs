using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Edity.PluginCore
{
    /// <summary>
    /// This interface provides a way to load edity plugins and have them hook into the
    /// main app's di.
    /// </summary>
    public interface IEdityPlugin
    {
        /// <summary>
        /// This funciton is called at the end of the Startup constructor. It will pass the configuration files for the
        /// site and the server.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        /// <param name="config">The application configuration.</param>
        /// <param name="serverConfig">The edity server configuration.</param>
        void OnStartup(IHostingEnvironment env, IConfigurationRoot config, IConfigurationRoot serverConfig);

        /// <summary>
        /// This function is called at the beginning of the ConfigureServices function on startup.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureServicesStart(IServiceCollection services);

        /// <summary>
        /// This function is called between ConfigureServicesStart and ConfigureServicesEnd as Mvc is being set up.
        /// </summary>
        /// <param name="builder">The mvc builder.</param>
        void ConfigureMvc(IMvcBuilder builder);

        /// <summary>
        /// This function is called at the end of the ConfigureServices function on startup.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureServicesEnd(IServiceCollection services);

        /// <summary>
        /// This function is called at the beginning of the Configure function on startup.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        void ConfigureStart(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory);

        /// <summary>
        /// This function is called at the end of the Configure function on startup.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        void ConfigureEnd(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory);
    }
}