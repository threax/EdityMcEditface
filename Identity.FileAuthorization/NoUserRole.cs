using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.FileAuthorization
{
    public class NoUserRole
    {
        public NoUserRole(String name)
        {
            this.Name = name;
        }

        public string Id { get; internal set; }

        public string NormalizedName { get; internal set; }

        public IList<Claim> Claims { get; private set; } = new List<Claim>();

        public string Name { get; internal set; }

        internal void AddClaim(Claim claim)
        {
            Claims.Add(claim);
        }

        internal void RemoveClaim(Claim claim)
        {
            Claims.Remove(claim);
        }
    }
}
