using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.HtmlRenderer.Filesystem;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Repositories
{
    public class DraftRepository : IDraftRepository
    {
        private IFileFinder fileFinder;
        private ITargetFileInfoProvider fileInfoProvider;
        private ProjectFinder projectFinder;
        private String pathBase;

        public DraftRepository(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider, ProjectFinder projectFinder, IPathBaseInjector pathBaseInjector)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
            this.projectFinder = projectFinder;
            this.pathBase = pathBaseInjector.PathBase;
        }

        /// <summary>
        /// Get the list of pages in draft.
        /// </summary>
        /// <returns>The list of drafted files.</returns>
        public async Task<DraftCollection> List(DraftQuery query)
        {
            DraftCollection collection;

            if (!String.IsNullOrEmpty(query.File)) //If file is not null, looking for specific file.
            {
                collection = new DraftCollection(query, 1, new Draft[] { await GetInfo(query.File) });
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
                        var fileInfo = fileInfoProvider.GetFileInfo(i.File, pathBase);
                        var pageSettings = fileFinder.GetProjectPageDefinition(fileInfo);
                        return new Draft(i, pageSettings.Title);
                    });

                collection = new DraftCollection(query, total, draftConvert);
            }

            return collection;
        }

        /// <summary>
        /// Get the draft info for a single file.
        /// </summary>
        /// <param name="file">The file to lookup.</param>
        /// <returns>The draft info for the file.</returns>
        public Task<Draft> GetInfo(String file)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(file, pathBase);

            var status = fileFinder.GetDraftStatus(fileInfo.DerivedFileName);
            var pageSettings = fileFinder.GetProjectPageDefinition(fileInfo);

            return Task.FromResult(new Draft(status, pageSettings.Title));
        }

        /// <summary>
        /// Update all pages and files with never drafted out of date status to their latest draft.
        /// </summary>
        /// <returns></returns>
        public Task SubmitAll()
        {
            var draftQuery = fileFinder.GetAllDraftables().Select(i => fileFinder.GetDraftStatus(i)).Where(i => i.Status != DraftStatus.UpToDate);

            foreach (var status in draftQuery)
            {
                var fileInfo = fileInfoProvider.GetFileInfo(status.File, pathBase);
                fileFinder.SendToDraft(fileInfo.DerivedFileName);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Mark the given file's latest revision as its draft.
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <returns></returns>
        public Task Submit(String file)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(file, pathBase);
            fileFinder.SendToDraft(fileInfo.DerivedFileName);
            return Task.FromResult(0);
        }
    }
}
