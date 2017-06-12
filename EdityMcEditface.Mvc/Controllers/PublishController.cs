using EdityMcEditface.Mvc.Models.Compiler;
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
    /// <summary>
    /// This controller publishes / compiles the static website.
    /// </summary>
    [Route("edity/[controller]/[action]")]
    [Authorize(Roles = Roles.Compile)]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    public class PublishController : Controller
    {
        public static class Rels
        {
            public const String PublishStatus = "PublishStatus";
            public const String Compile = "Compile";
        }

        private IPublishRepository publishRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publishRepo">The publish repository.</param>
        public PublishController(IPublishRepository publishRepo)
        {
            this.publishRepo = publishRepo;
        }

        /// <summary>
        /// Get the current status of the compiler.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HalRel(Rels.PublishStatus)]
        public CompilerStatus Status()
        {
            return publishRepo.Status();
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        /// <returns>The time statistics when the compilation is complete.</returns>
        [HttpPost]
        [HalRel(Rels.Compile)]
        public Task<CompilerResult> Compile()
        {
            return publishRepo.Compile();
        }
    }
}
