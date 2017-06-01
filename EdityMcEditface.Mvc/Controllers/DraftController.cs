using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models;
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
    public class DraftController : Controller
    {
        public static class Rels
        {
            public const String List = "ListDrafts";
            public const String SubmitLatestDraft = "SubmitLatestDraft";
        }

        IFileFinder fileFinder;
        ITargetFileInfoProvider fileInfoProvider;
        ProjectFinder projectFinder;

        public DraftController(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider, ProjectFinder projectFinder)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
            this.projectFinder = projectFinder;
        }

        /// <summary>
        /// Get the list of files in draft.
        /// </summary>
        /// <returns>The list of drafted files.</returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public Task<DraftCollection> List([FromQuery]DraftQuery query)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(query.File, HttpContext.Request.PathBase);

            var collection = new DraftCollection(query, 1, new Draft[] { new Draft()
            {
                File = fileInfo.DerivedFileName
            } });

            return Task.FromResult(collection);
        }

        /// <summary>
        /// Mark the given file's latest revision as its draft.
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <returns></returns>
        [HttpPut("{File}")]
        [HalRel(Rels.SubmitLatestDraft)]
        public Task Put(String file)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(file, HttpContext.Request.PathBase);
            fileFinder.SendToDraft(fileInfo.DerivedFileName);
            return Task.FromResult(0);
        }
    }
}
