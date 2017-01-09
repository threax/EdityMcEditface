using Edity.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Edity.NoAuth
{
    public class NoAuthPlugin : EdityPlugin
    {
        public override void ConfigureServicesStart(IServiceCollection services)
        {
            base.ConfigureServicesStart(services);
        }

        public override void ConfigureMvc(IMvcBuilder builder)
        {
            builder.AddApplicationPart(this.GetType().Assembly);
            base.ConfigureMvc(builder);
        }

        public override void ConfigureStart(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            base.ConfigureStart(app, env, loggerFactory);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = "/edity/auth/login",
                LogoutPath = "/edity/auth/logout"
            });
        }
    }
}
