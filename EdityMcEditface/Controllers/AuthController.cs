using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.Auth;
using Identity.NoSqlAuthorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [Route("edity/[controller]/[action]")]
    public class AuthController : Controller
    {
        private UserManager<EdityNoSqlUser> userManager;
        private SignInManager<EdityNoSqlUser> signInManager;
        private RoleManager<NoSqlRole> roleManager;

        public AuthController(UserManager<EdityNoSqlUser> userManager, SignInManager<EdityNoSqlUser> signInManager, RoleManager<NoSqlRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn([FromServices] FileFinder fileFinder, String returnUrl)
        {
            var templateEnvironment = new TemplateEnvironment(Request.Path, fileFinder.Project);
            var dr = new HtmlDocumentRenderer(templateEnvironment);
            var pageStack = new PageStack(templateEnvironment, fileFinder);
            pageStack.pushLayout("login.html");
            var document = dr.getDocument(pageStack.Pages);
            return Content(document.DocumentNode.OuterHtml, new MediaTypeHeaderValue("text/html"));
        }

        [HttpPost]
        [AllowAnonymous]
        [AutoValidate("Invalid Login")]
        public async Task<IActionResult> LogIn([FromBody] LoginModel loginModel)
        {
            var result = await signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, false, false);
            if (result.Succeeded)
            {
                return StatusCode(200);
            }
            else
            {
                throw new ErrorResultException("Invalid login", System.Net.HttpStatusCode.BadRequest);
            }
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
        public async Task<IActionResult> LogOut([FromServices] IAntiforgery antiforgery)
        {
            await signInManager.SignOutAsync();
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Delete(tokens.HeaderName);
            return StatusCode(200);
        }

        [HttpPost]
        [AllowAnonymous]
        [AutoValidate("Invalid Registration Data")]
        public async Task<IActionResult> Register([FromBody] RegisterModel loginModel)
        {
            var user = new EdityNoSqlUser()
            {
                Name = loginModel.UserName,
                DisplayName = loginModel.DisplayName,
                Email = loginModel.Email
            };

            var idResult = await userManager.CreateAsync(user, loginModel.Password);
            if (idResult.Succeeded)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Role, Roles.EditPages),
                    new Claim(ClaimTypes.Role, Roles.Compile),
                    new Claim(ClaimTypes.Role, Roles.UploadAnything),
                    new Claim(ClaimTypes.Role, Roles.Shutdown),
                };

                idResult = await userManager.AddClaimsAsync(user, claims);
                if (idResult.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return StatusCode(200);
                }
                else
                {
                    throw new ErrorResultException("Error registering new user, cannot add claims.");
                }
            }
            else
            {
                throw new ErrorResultException("Error registering new user, cannot create user.");
            }
        }
    }
}
