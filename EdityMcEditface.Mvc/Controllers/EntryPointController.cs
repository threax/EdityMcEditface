using EdityMcEditface.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer)]
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
