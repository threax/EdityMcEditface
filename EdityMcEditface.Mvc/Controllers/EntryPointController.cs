using EdityMcEditface.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("")]
    [ResponseCache(NoStore = true)]
    public class EntryPointController : Controller
    {
        public class Rels
        {
            public const String Get = "GetEntryPoint";
        }

        [HttpGet]
        [HalRel(Rels.Get)]
        public EntryPoint Get()
        {
            return new EntryPoint();
        }
    }
}
