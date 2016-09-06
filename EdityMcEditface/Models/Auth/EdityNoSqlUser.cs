using Identity.NoSqlAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Auth
{
    public class EdityNoSqlUser : NoSqlUser
    {
        public String DisplayName { get; set; }
    }
}
