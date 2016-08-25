using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;

namespace Identity.NoSqlAuthorization
{
    public class NoSqlRoleStore<TRole> : IRoleClaimStore<TRole> where TRole : NoSqlRole
    {
        private List<TRole> roles;
        private IAuthSerializer<TRole> serializer;

        public NoSqlRoleStore(IAuthSerializer<TRole> serializer)
        {
            this.serializer = serializer;
            roles = new List<TRole>(serializer.Load());
        }

        public void Dispose()
        {

        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            role.AddClaim(claim);
            return Task.FromResult(0);
        }

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            roles.Add(role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            roles.Remove(role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            foreach (var role in roles)
            {
                if (role.Id == roleId)
                {
                    return Task.FromResult(role);
                }
            }
            return Task.FromResult(default(TRole));
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            foreach (var role in roles)
            {
                if (role.NormalizedName == normalizedRoleName)
                {
                    return Task.FromResult(role);
                }
            }
            return Task.FromResult(default(TRole));
        }

        public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(role.Claims);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            role.RemoveClaim(claim);
            return Task.FromResult(0);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            serializer.Save(roles);
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
