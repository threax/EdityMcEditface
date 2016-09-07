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
    [Route("edity/[controller]")]
    [Authorize(Roles = Roles.Shutdown)]
    public class ShutdownController : Controller
    {
        public ShutdownController()
        {
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Index()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
#endif