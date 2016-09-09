using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Config
{
    public class EditySettings
    {
        public bool ReadFromCurrentDirectory { get; set; }

        public String UsersFile { get; set; }

        public bool DetailedErrors { get; set; }

        public bool SecureCookies { get; set; }
    }
}
