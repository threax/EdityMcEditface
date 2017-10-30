using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Repositories;
using LibGit2Sharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer, Roles = Roles.EditPages)]
    public class SyncController : Controller
    {
        public static class Rels
        {
            public const String BeginSync = "BeginSync";
            public const String Push = "Push";
            public const String Pull = "Pull";
        }

        private ISyncRepository syncRepo;

        public SyncController(ISyncRepository syncRepo)
        {
            this.syncRepo = syncRepo;
        }

        [HttpGet]
        [HalRel(Rels.BeginSync)]
        public Task<SyncInfo> Get()
        {
            return this.syncRepo.SyncInfo();
        }

        [HttpPut("[action]")]
        [HalRel(Rels.Push)]
        public Task Push()
        {
            return this.syncRepo.Push();
        }

        [HttpPut("[action]")]
        [HalRel(Rels.Pull)]
        public Task Pull([FromServices]Signature signature)
        {
            return this.syncRepo.Pull(signature);
        }
    }
}
