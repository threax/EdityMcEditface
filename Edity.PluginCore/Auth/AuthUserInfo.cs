using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Edity.PluginCore
{
    public class AuthUserInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthUserInfo(IHttpContextAccessor httpContextAccessor) : base()
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal ClaimsPrincipal
        {
            get
            {
                return _httpContextAccessor.HttpContext.User;
            }
        }

        public String UserName
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }
    }
}
