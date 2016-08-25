using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;

namespace Identity.NoSqlAuthorization
{
    public class NoUserStore<TUser> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>
        where TUser : NoUserUser
    {
        private List<TUser> users;
        private IAuthSerializer<TUser> serializer;

        public NoUserStore(IAuthSerializer<TUser> serializer)
        {
            this.serializer = serializer;
            users = new List<TUser>(serializer.Load());
        }

        public void Dispose()
        {

        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.AddClaims(claims);
            return Task.FromResult(0);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            user.AddRole(roleName);
            return Task.FromResult(0);
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            users.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            users.Remove(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            foreach (var userInstance in users)
            {
                if (userInstance.NormalizedEmail == normalizedEmail)
                {
                    return Task.FromResult(userInstance);
                }
            }
            return Task.FromResult(default(TUser));
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            foreach (var userInstance in users)
            {
                if (userInstance.Id == userId)
                {
                    return Task.FromResult(userInstance);
                }
            }
            return Task.FromResult(default(TUser));
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(TUser));
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            foreach (var userInstance in users)
            {
                if (userInstance.NormalizedName == normalizedUserName)
                {
                    return Task.FromResult(userInstance);
                }
            }
            return Task.FromResult(default(TUser));
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<Claim>)user.Claims);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(DateTimeOffset?));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> list = new List<UserLoginInfo>();
            return Task.FromResult(list);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<string>)user.Roles);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Name);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var list = new List<TUser>();
            foreach (var userInstance in users)
            {
                if (userInstance.HasClaim(claim))
                {
                    list.Add(userInstance);
                }
            }
            return Task.FromResult((IList<TUser>)list);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var list = new List<TUser>();

            foreach (var user in users)
            {
                if (user.HasRole(roleName))
                {
                    list.Add(user);
                }
            }

            return Task.FromResult((IList<TUser>)list);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.HasRole(roleName));
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.RemoveClaims(claims);
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            user.RemoveRole(roleName);
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            user.ReplaceClaim(claim, newClaim);
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            serializer.Save(users);
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
