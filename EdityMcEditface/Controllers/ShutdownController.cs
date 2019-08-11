using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    /// <summary>
    /// This controller will shutdown Edity McEditface.
    /// </summary>
    [Route("edity/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer, Roles = LocalEdityRoles.Shutdown)]
    [ResponseCache(NoStore = true)]
    public class ShutdownController : Controller
    {
        /// <summary>
        /// Consturctor
        /// </summary>
        public ShutdownController()
        {

        }

        /// <summary>
        /// Stop the Edity McEditface process.
        /// </summary>
        public ActionResult Shutdown()
        {
            HttpContext.Response.OnCompleted(() =>
            {
                Process.GetCurrentProcess().Kill();
                return Task.FromResult(0);
            });
            return Content("Edity McEdtiface Shut Down.", "text/plain");
        }
    }
}