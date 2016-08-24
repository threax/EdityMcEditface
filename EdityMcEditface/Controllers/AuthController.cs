using EdityMcEditface.ErrorHandling;
using EdityMcEditface.Models.Auth;
using Identity.FileAuthorization;
using Microsoft.AspNetCore.Identity;
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
        private UserManager<NoUserUser> userManager;
        private SignInManager<NoUserUser> signInManager;
        private RoleManager<NoUserRole> roleManager;

        public AuthController(UserManager<NoUserUser> userManager, SignInManager<NoUserUser> signInManager, RoleManager<NoUserRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> LogIn(String returnUrl)
        {
            //temp, create the user first
            var user = new NoUserUser()
            {
                Name = "Anon",
                Email = "anon@spcollege.edu"
            };
            await userManager.CreateAsync(user, "Password@43");

            var claims = new[] {
                    new Claim(ClaimTypes.Role, Roles.EditPages),
                    new Claim(ClaimTypes.Role, Roles.Compile),
                    new Claim(ClaimTypes.Role, Roles.UploadAnything),
                    new Claim(ClaimTypes.Role, Roles.Shutdown),
                };

            await userManager.AddClaimsAsync(user, claims);

            var roles = new[] {
                    Roles.EditPages,
                    Roles.Compile,
                    Roles.UploadAnything,
                    Roles.Shutdown,
                };
            await userManager.AddToRolesAsync(user, roles);

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(new NoUserRole(role));
            }
            //end temp

            var result = await signInManager.PasswordSignInAsync("Anon", "Password@43", false, false);
            if (result.Succeeded)
            {
                return SafeRedirect(ref returnUrl);
            }
            else
            {
                throw new ErrorResultException("Invalid login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LogOut(String returnUrl)
        {
            await signInManager.SignOutAsync();
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
