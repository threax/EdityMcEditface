using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    public class AccessDeniedController : Controller
    {
        public String Index()
        {
            return "Access Denied";
        }
    }
}
