using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.FileAuthorization
{
    public class NoUserUser
    {
        public NoUserUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Email { get; set; }
        public string Name { get; set; }
        public List<Claim> Claims { get; private set; } = new List<Claim>();
        public List<String> Roles { get; private set; } = new List<string>();

        internal String PasswordHash { get; set; }

        internal String NormalizedEmail { get; set; }

        internal String NormalizedName { get; set; }

        internal string Id { get; private set; }

        internal bool HasClaim(Claim claim)
        {
            return Claims.Contains(claim);
        }

        internal bool HasRole(string roleName)
        {
            return Roles.Contains(roleName);
        }

        internal void RemoveClaims(IEnumerable<Claim> claims)
        {
            foreach(var claim in claims)
            {
                this.Claims.Remove(claim);
            }
        }

        internal void AddClaims(IEnumerable<Claim> claims)
        {
            this.Claims.AddRange(claims);
        }

        internal void RemoveRole(String roleName)
        {
            Roles.Remove(roleName);
        }

        internal void AddRole(string roleName)
        {
            Roles.Add(roleName);
        }

        internal void ReplaceClaim(Claim claim, Claim newClaim)
        {
            var index = Claims.IndexOf(claim);
            if(index != -1)
            {
                Claims.RemoveAt(index);
                Claims.Insert(index, newClaim);
            }
        }
    }
}
