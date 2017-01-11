﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Auth
{
    /// <summary>
    /// A default user info that reads the user identity name and makes up an email.
    /// </summary>
    public class DefaultUserInfo : IUserInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultUserInfo(IHttpContextAccessor httpContextAccessor) : base()
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// The unique user name for this account. Must be unique across
        /// all users of the system.
        /// </summary>
        public String UniqueUserName
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }

        /// <summary>
        /// The pretty, displayable name for the user.
        /// </summary>
        public String PrettyUserName
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }

        /// <summary>
        /// The email for the user. Always @nowhere.com for the DefaultUserInfo, if you need customization
        /// implement your own IUserInfo.
        /// </summary>
        public String Email
        {
            get
            {
                return UniqueUserName + "@nowhere.com";
            }
        }
    }
}
