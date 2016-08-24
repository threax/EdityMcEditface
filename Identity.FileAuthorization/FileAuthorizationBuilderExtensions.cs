using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Identity.FileAuthorization
{
    public static class FileAuthorizationBuilderExtensions
    {
        public static IdentityBuilder AddNoAuthorization<TUser, TRole>(this IdentityBuilder builder) 
            where TUser : NoUserUser 
            where TRole : NoUserRole
        {
            GetDefaultServices<TUser, TRole>(builder);
            return builder;
        }

        private static void GetDefaultServices<TUser, TRole>(IdentityBuilder builder) 
            where TUser : NoUserUser
            where TRole : NoUserRole
        {
            builder.Services.AddScoped(typeof(IUserStore<TUser>), (sp) =>
            {
                var store =  new NoUserStore<TUser>();
                return store;
            });
            builder.Services.AddScoped(typeof(IRoleStore<TRole>), (sp) =>
            {
                return new NoUserRoleStore<TRole>();
            });
        }
    }
}
