using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Auth
{
    public class AntiforgeryToken
    {
        public String HeaderName { get; set; }

        public String RequestToken { get; set; }
    }
}
