#if LOCAL_RUN_ENABLED
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
    /// This controller will shutdown Edity McEditface, this only works if the program was
    /// compiled with LOCAL_RUN_ENABLED.
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
        [HttpPost]
        public void Shutdown()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
#endif