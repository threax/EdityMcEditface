using EdityMcEditface.Mvc.Models.Branch;
using EdityMcEditface.Mvc.Repositories;
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
    public class BranchController : Controller
    {
        public static class Rels
        {
            public const String List = "ListBranches";
            public const String Add = "AddBranch";
            public const String Checkout = "CheckoutBranch";
        }

        private IBranchRepository branchRepo;

        public BranchController(IBranchRepository branchRepo)
        {
            this.branchRepo = branchRepo;
        }

        [HttpGet]
        [HalRel(Rels.List)]
        public BranchCollection List()
        {
            return branchRepo.List();
        }

        [HttpPost]
        [HalRel(Rels.Add)]
        [Authorize(Roles = Roles.AddBranch)]
        public void Add(BranchInput input)
        {
            branchRepo.Add(input.Name);
        }

        [HttpPost("[action]/{FriendlyName}")]
        [HalRel(Rels.Checkout)]
        public void Checkout(String friendlyName, [FromServices] LibGit2Sharp.Signature sig)
        {
            branchRepo.Checkout(friendlyName, sig);
        }
    }
}
