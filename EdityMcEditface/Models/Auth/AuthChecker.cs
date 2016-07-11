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
                return Config.CookieAuthenticationSchemeName;
            }
        }

        public ClaimsPrincipal ClaimsPrincipal
        {
            get
            {
                var identity = new ClaimsIdentity(new AuthIdentity("OnlyUser", "SingleUserAuthentication"));
                return new ClaimsPrincipal(identity);
            }
        }
    }
}
