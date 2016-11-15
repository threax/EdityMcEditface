using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.Auth;
using EdityMcEditface.Models.Config;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Threax.AspNetCore.ExceptionToJson;

#if LOCAL_RUN_ENABLED

namespace EdityMcEditface.Controllers
{
    /// <summary>
    /// This controller will automatically log in a user when they hit the login function.
    /// This only works if LOCAL_RUN_ENABLED was enabled when the program was compiled.
    /// </summary>
    [Authorize]
    public class NoAuthController : Controller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NoAuthController()
        {
            
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromServices] EditySettings settings, String returnUrl)
        {

            var identity = new ClaimsIdentity(AllClaims(settings), "Cookies", "name", "role");
            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(identity));
            

            return SafeRedirect(ref returnUrl);
        }

        /// <summary>
        /// The claims for the user logging in.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Claim> AllClaims(EditySettings settings)
        {
            yield return new Claim("name", settings.NoAuthUser);
            yield return new Claim("role", Roles.EditPages);
            yield return new Claim("role", Roles.Compile);
            yield return new Claim("role", Roles.UploadAnything);
#if LOCAL_RUN_ENABLED
            yield return new Claim("role", Roles.Shutdown);
#endif
        }

        /// <summary>
        /// Get the antiforgery token.
        /// </summary>
        /// <param name="antiforgery"></param>
        /// <returns></returns>
        [HttpPost]
        public AntiforgeryToken AntiforgeryToken([FromServices] IAntiforgery antiforgery)
        {
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append(tokens.HeaderName, tokens.RequestToken, new CookieOptions() { HttpOnly = false });
            return new AntiforgeryToken()
            {
                HeaderName = tokens.HeaderName,
                RequestToken = tokens.RequestToken
            };
        }

        /// <summary>
        /// Log out.
        /// </summary>
        /// <param name="antiforgery"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogOut([FromServices] IAntiforgery antiforgery, String returnUrl)
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");

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
            catch (UriFormatException)
            {
                returnUrl = "~/";
            }

            return Redirect(returnUrl);
        }
    }
}

#endif