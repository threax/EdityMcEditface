using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Auth
{
    public class AuthChecker
    {
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public String AuthenticationScheme
        {
            get
            {
                return AuthenticationConfig.CookieAuthenticationSchemeName;
            }
        }

        public ClaimsPrincipal ClaimsPrincipal
        {
            get
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Role, Roles.EditPages),
                    new Claim(ClaimTypes.Role, Roles.Compile),
                    new Claim(ClaimTypes.Role, Roles.UploadAnything),
                };
                var identity = new ClaimsIdentity(new AuthIdentity("Anon", "AnonAuthentication"), claims);
                return new ClaimsPrincipal(identity);
            }
        }
    }
}
