using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Identity.NoSqlAuthorization
{
    public static class NoSqlAuthorizationBuilderExtensions
    {
        public static IdentityBuilder AddNoSqlAuthorization<TUser, TRole>(this IdentityBuilder builder) 
            where TUser : NoSqlUser 
            where TRole : NoSqlRole
        {
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

            return builder;
        }

        public static IdentityBuilder AddJsonSerializers<TUser, TRole>(this IdentityBuilder builder, String usersFile = "users.json", String rolesFile = "roles.json")
            where TUser : NoSqlUser
            where TRole : NoSqlRole
        {
            builder.Services.AddScoped(typeof(IAuthSerializer<TUser>), (sp) =>
            {
                return new JsonSimpleSerializer<TUser>(usersFile);
            });

            builder.Services.AddScoped(typeof(IAuthSerializer<TRole>), (sp) =>
            {
                return new JsonSimpleSerializer<TRole>(rolesFile);
            });

            return builder;
        }

        public static IdentityBuilder AddJsonSerializers<TUser, TRole>(this IdentityBuilder builder, String usersFile, IAuthSerializer<TRole> roleSerializer)
            where TUser : NoSqlUser
            where TRole : NoSqlRole
        {
            builder.Services.AddScoped(typeof(IAuthSerializer<TUser>), (sp) =>
            {
                return new JsonSimpleSerializer<TUser>(usersFile);
            });

            builder.Services.AddScoped(typeof(IAuthSerializer<TRole>), (sp) =>
            {
                return roleSerializer;
            });

            return builder;
        }

        public static IdentityBuilder AddJsonSerializers<TUser, TRole>(this IdentityBuilder builder, String usersFile, Func<IEnumerable<TRole>> roleProvider)
            where TUser : NoSqlUser
            where TRole : NoSqlRole
        {
            builder.Services.AddScoped(typeof(IAuthSerializer<TUser>), (sp) =>
            {
                return new JsonSimpleSerializer<TUser>(usersFile);
            });

            builder.Services.AddScoped(typeof(IAuthSerializer<TRole>), (sp) =>
            {
                return new InMemoryStaticAuthSerializer<TRole>(roleProvider);
            });

            return builder;
        }

        public static IdentityBuilder AddSerializers<TUser, TRole>(this IdentityBuilder builder, IAuthSerializer<TUser> userSerializer, IAuthSerializer<TRole> roleSerializer)
            where TUser : NoSqlUser
            where TRole : NoSqlRole
        {
            builder.Services.AddScoped(typeof(IAuthSerializer<TUser>), (sp) =>
            {
                return userSerializer;
            });

            builder.Services.AddScoped(typeof(IAuthSerializer<TRole>), (sp) =>
            {
                return roleSerializer;
            });

            return builder;
        }

        public static IdentityBuilder AddSerializers<TUser, TRole>(this IdentityBuilder builder, Func<IEnumerable<TUser>> userProvider, Func<IEnumerable<TRole>> roleProvider)
            where TUser : NoSqlUser
            where TRole : NoSqlRole
        {
            builder.Services.AddScoped(typeof(IAuthSerializer<TUser>), (sp) =>
            {
                return new InMemoryStaticAuthSerializer<TUser>(userProvider);
            });

            builder.Services.AddScoped(typeof(IAuthSerializer<TRole>), (sp) =>
            {
                return new InMemoryStaticAuthSerializer<TRole>(roleProvider);
            });

            return builder;
        }
    }
}
