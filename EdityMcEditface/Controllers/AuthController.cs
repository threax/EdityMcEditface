using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.Auth;
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

namespace EdityMcEditface.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        public AuthController()
        {
            
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(String returnUrl)
        {

            var identity = new ClaimsIdentity(AllClaims(), "Cookies", "name", "role");
            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(identity));
            

            return SafeRedirect(ref returnUrl);
        }

        private IEnumerable<Claim> AllClaims()
        {
            yield return new Claim("name", "OnlyUser");
            yield return new Claim("role", Roles.EditPages);
            yield return new Claim("role", Roles.Compile);
            yield return new Claim("role", Roles.UploadAnything);
#if LOCAL_RUN_ENABLED
            yield return new Claim("role", Roles.Shutdown);
#endif
        }

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
