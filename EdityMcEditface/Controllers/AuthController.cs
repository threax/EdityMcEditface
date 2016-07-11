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
        public async Task<IActionResult> LogIn(String returnUrl)
        {
            if (authChecker.IsValid)
            {
                await HttpContext.Authentication.SignInAsync(authChecker.AuthenticationScheme, authChecker.ClaimsPrincipal);
            }

            return SafeRedirect(ref returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut(String returnUrl)
        {
            await HttpContext.Authentication.SignOutAsync(authChecker.AuthenticationScheme);

            return SafeRedirect(ref returnUrl);
        }

        private IActionResult SafeRedirect(ref string returnUrl)
        {
            //See if we have anything
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }

            //Make sure they aren't trying to redirect to another website
            try
            {
                Uri uri = new Uri(returnUrl, UriKind.Relative);
            }
            catch(UriFormatException)
            {
                returnUrl = "~/";
            }

            return Redirect(returnUrl);
        }
    }
}
