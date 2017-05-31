using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Prepublish;
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
    public class PrepublishController : Controller
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
            public const String Find = "FindPrepublishedFile";
            public const String Prepublish = "SendToPrepublish";
        }

        IFileFinder fileFinder;
        ITargetFileInfoProvider fileInfoProvider;
        ProjectFinder projectFinder;

        public PrepublishController(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider, ProjectFinder projectFinder)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
            this.projectFinder = projectFinder;
        }

        /// <summary>
        /// Get the list of branches. Not paged.
        /// </summary>
        /// <returns>The list of branches.</returns>
        [HttpGet]
        [HalRel(Rels.Find)]
        public Task<PrepublishFileInfo> Find([FromQuery]PrepublishFileQuery query)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(query.File, HttpContext.Request.PathBase);

            return Task.FromResult(new PrepublishFileInfo()
            {
                File = fileInfo.DerivedFileName
            });
        }

        /// <summary>
        /// Publish the given file
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <returns></returns>
        [HttpPut("{File}")]
        [HalRel(Rels.Prepublish)]
        public Task Put(String file)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(file, HttpContext.Request.PathBase);
            fileFinder.PrepublishPage(fileInfo.DerivedFileName);
            return Task.FromResult(0);
        }
    }
}
