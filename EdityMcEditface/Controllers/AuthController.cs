using EdityMcEditface.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]/[action]")]
    public class AuthController : Controller
    {
        private AuthChecker authChecker;

        public AuthController(AuthChecker authChecker)
        {
            this.authChecker = authChecker;
        }

        [HttpGet]
        public async Task<IActionResult> LogIn()
        {
            if (authChecker.IsValid)
            {
                await HttpContext.Authentication.SignInAsync(authChecker.AuthenticationScheme, authChecker.ClaimsPrincipal);
            }
            return Redirect("~/");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.Authentication.SignOutAsync(authChecker.AuthenticationScheme);

            return Redirect("~/");
        }
    }
}
