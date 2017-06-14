using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize(Roles = Roles.EditPages)]
    public class MergeController : Controller
    {
        public static class Rels
        {
            public const String GetMergeInfo = "GetMergeInfo";
            public const String Resolve = "Resolve";
        }

        IMergeRepository mergeRepo;

        public MergeController(IMergeRepository mergeRepo)
        {
            this.mergeRepo = mergeRepo;
        }

        /// <summary>
        /// Get the info for a merge for a file.
        /// </summary>
        /// <param name="query">The query for the file to lookup.</param>
        /// <returns></returns>
        [HttpGet]
        [HalRel(Rels.GetMergeInfo)]
        public MergeInfo MergeInfo([FromQuery] MergeQuery query)
        {
            return mergeRepo.MergeInfo(query.File);
        }

        /// <summary>
        /// Resolve the conflicts on the file.
        /// </summary>
        /// <param name="file">The file to resolve.</param>
        /// <param name="args">The args with the file content.</param>
        /// <returns></returns>
        [HttpPost("[action]/{*File}")]
        [HalRel(Rels.Resolve)]
        public Task Resolve(String file, [FromForm] ResolveMergeArgs args)
        {
            return mergeRepo.Resolve(file, args.Content);
        }
    }
}
