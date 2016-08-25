﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Identity.NoSqlAuthorization
{
    public static class FileAuthorizationBuilderExtensions
    {
        public static IdentityBuilder AddNoAuthorization<TUser, TRole>(this IdentityBuilder builder) 
            where TUser : NoSqlUser 
            where TRole : NoSqlRole
        {
            GetDefaultServices<TUser, TRole>(builder);
            return builder;
        }

        private static void GetDefaultServices<TUser, TRole>(IdentityBuilder builder) 
            where TUser : NoSqlUser
            where TRole : NoSqlRole
        {
            builder.Services.AddScoped(typeof(IAuthSerializer<TUser>), (sp) =>
            {
                return new JsonSimpleSerializer<TUser>("users.json");
            });

            builder.Services.AddScoped(typeof(IAuthSerializer<TRole>), (sp) =>
            {
                return new JsonSimpleSerializer<TRole>("roles.json");
            });

            builder.Services.AddScoped(typeof(IUserStore<TUser>), (sp) =>
            {
                var serializer = sp.GetRequiredService<IAuthSerializer<TUser>>();
                return new NoSqlUserStore<TUser>(serializer);
            });

            builder.Services.AddScoped(typeof(IRoleStore<TRole>), (sp) =>
            {
                var serializer = sp.GetRequiredService<IAuthSerializer<TRole>>();
                return new NoSqlRoleStore<TRole>(serializer);
            });
        }
    }
}
