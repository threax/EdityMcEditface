using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Auth
{
    class AuthIdentity : IIdentity
    {
        public AuthIdentity(String name, String authenticationType)
        {
            this.Name = name;
            this.AuthenticationType = authenticationType;
        }

        public string AuthenticationType { get; private set; }

        public string Name { get; private set; }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }
    }
}
