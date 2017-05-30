using EdityMcEditface.Mvc.Models.Branch;
using EdityMcEditface.Mvc.Models.Page;
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
    public class BranchController : Controller
    {
        /// <summary>
        /// Each controller should define a Rels class inside of it. Note that each rel across the system
        /// should be unique, so name them with an appropriate prefix. Right now these rel names are used
        /// by the codegen to make functions, which is why they need to be unique (technically only unique
        /// across any endpoints that have the same rels, but globally is a good enough approximiation of this,
        /// I hope to fix this in the future.
        /// </summary>
        public static class Rels
        {
            public const String List = "ListBranches";
            public const String SetBranch = "SetBranch";
        }

        ProjectFinder projectFinder;

        public BranchController(ProjectFinder projectFinder)
        {
            this.projectFinder = projectFinder;
        }

        /// <summary>
        /// Get the list of branches. Not paged.
        /// </summary>
        /// <returns>The list of branches.</returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public Task<BranchViewCollection> Get()
        {
            return this.projectFinder.GetBranches();
        }

        /// <summary>
        /// Set the branch that is currently active.
        /// </summary>
        /// <param name="name">The name of the branch to make active</param>
        /// <returns></returns>
        [HttpPut("Name")]
        [HalRel(Rels.SetBranch)]
        public Task Put(String name)
        {
            return Task.FromResult(0);
        }
    }
}
