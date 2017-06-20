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
    [Route("edity/[controller]")]
    [Authorize(Roles = Roles.Compile)]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    public class PublishController : Controller
    {
        public static class Rels
        {
            public const String BeginPublish = "BeginPublish";
            public const String Compile = "Compile";
            public const String Progress = "Progress";
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
        [HalRel(Rels.BeginPublish)]
        public Task<PublishEntryPoint> Get([FromServices] ISyncRepository syncRepo)
        {
            return publishRepo.GetPublishInfo(syncRepo);
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        /// <returns>The time statistics when the compilation is complete.</returns>
        [HttpPost("[action]")]
        [HalRel(Rels.Compile)]
        public CompileProgress Compile()
        {
            publishRepo.Compile();
            return publishRepo.Progress();
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        /// <returns>The time statistics when the compilation is complete.</returns>
        [HttpPost("[action]")]
        [HalRel(Rels.Progress)]
        public CompileProgress Progress()
        {
            return publishRepo.Progress();
        }
    }
}
