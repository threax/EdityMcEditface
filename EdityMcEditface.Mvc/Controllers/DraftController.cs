using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.HtmlRenderer.Filesystem;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(Roles = Roles.CreateDrafts)]
    public class DraftController : Controller
    {
        public static class Rels
        {
            public const String List = "ListDrafts";
            public const String SubmitLatestDraft = "SubmitLatestDraft";
            public const String SubmitAllDrafts = "SubmitAllDrafts";
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
        /// Get the list of pages in draft.
        /// </summary>
        /// <returns>The list of drafted files.</returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public Task<DraftCollection> List([FromQuery]DraftQuery query)
        {
            DraftCollection collection;

            if (!String.IsNullOrEmpty(query.File) || HttpContext.Request.Query.Any(i => "file".Equals(i.Key, StringComparison.OrdinalIgnoreCase))) //Check query, if file was in it we are looking for index
            {
                var fileInfo = fileInfoProvider.GetFileInfo(query.File, HttpContext.Request.PathBase);

                collection = new DraftCollection(query, 1, new Draft[] { new Draft()
                {
                    File = fileInfo.DerivedFileName
                } });
            }
            else
            {
                var draftQuery = fileFinder.GetAllDraftables();
                var total = draftQuery.Count();
                var draftConvert = draftQuery
                    .Skip(query.SkipTo(total))
                    .Take(query.Limit)
                    .Select(i => fileFinder.GetDraftStatus(i))
                    .Select(i =>
                    {
                        var fileInfo = fileInfoProvider.GetFileInfo(i.File, HttpContext.Request.PathBase);
                        var pageSettings = fileFinder.GetProjectPageDefinition(fileInfo);
                        return new Draft()
                        {
                            File = i.File,
                            LastUpdate = i.LastUpdate,
                            Status = i.Status,
                            Title = pageSettings.Title
                        };
                    });

                collection = new DraftCollection(query, total, draftConvert);
            }

            return Task.FromResult(collection);
        }

        /// <summary>
        /// Update all pages and files with never drafted out of date status to their latest draft.
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        [HalRel(Rels.SubmitAllDrafts)]
        public Task SubmitAll()
        {
            var draftQuery = fileFinder.GetAllDraftables().Select(i => fileFinder.GetDraftStatus(i)).Where(i => i.Status != DraftStatus.UpToDate);

            foreach(var status in draftQuery)
            {
                var fileInfo = fileInfoProvider.GetFileInfo(status.File, HttpContext.Request.PathBase);
                fileFinder.SendToDraft(fileInfo.DerivedFileName);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Mark the given file's latest revision as its draft.
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <returns></returns>
        [HttpPut("{*File}")]
        [HalRel(Rels.SubmitLatestDraft)]
        public Task Put(String file)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(file, HttpContext.Request.PathBase);
            fileFinder.SendToDraft(fileInfo.DerivedFileName);
            return Task.FromResult(0);
        }
    }
}
