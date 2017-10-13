using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.FileRepository;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Repositories
{
    public class PageRepository : IPageRepository
    {
        private IFileFinder fileFinder;
        private ITargetFileInfoProvider fileInfoProvider;
        private String pathBase;
        private IFileVerifier fileVerifier;

        public PageRepository(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider, IPathBaseInjector pathBaseInjector, IFileVerifier fileVerifier)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
            this.pathBase = pathBaseInjector.PathBase;
            this.fileVerifier = fileVerifier;
        }

        /// <summary>
        /// Get the current page settings.
        /// </summary>
        /// <param name="page">The name of the file to lookup.</param>
        /// <returns>The PageSettings for the file.</returns>
        public PageSettings GetSettings(String page)
        {
            var targetFile = fileInfoProvider.GetFileInfo(page, pathBase);
            var definition = fileFinder.GetProjectPageDefinition(targetFile);
            String title;
            if (!definition.Vars.TryGetValue("title", out title))
            {
                title = "Untitled";
            }
            return new PageSettings()
            {
                Title = title,
                Visible = !definition.Hidden,
                FilePath = page
            };
        }

        /// <summary>
        /// Update the settings for the page.
        /// </summary>
        /// <param name="page">The file who's pages to upload.</param>
        /// <param name="settings">The page settings to set.</param>
        public void UpdateSettings(String page, PageSettings settings)
        {
            var targetFile = fileInfoProvider.GetFileInfo(page, pathBase);
            var definition = fileFinder.GetProjectPageDefinition(targetFile);
            definition.Vars["title"] = settings.Title;
            definition.Hidden = !settings.Visible;
            fileFinder.SavePageDefinition(definition, targetFile);
        }

        /// <summary>
        /// Save a page.
        /// </summary>
        /// <param name="page">The file to save.</param>
        /// <param name="content">The file content.</param>
        public async Task Save(String page, IFormFile content)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(page, pathBase);
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot update project files with the save function.");
            }
            using (var contentStream = content.OpenReadStream())
            {
                fileVerifier.Validate(contentStream, fileInfo.DerivedFileName, content.ContentType);
                using (Stream stream = fileFinder.WriteFile(fileInfo.DerivedFileName))
                {
                    await contentStream.CopyToAsync(stream);
                }
            }
        }

        /// <summary>
        /// Delete a page.
        /// </summary>
        /// <param name="page">The name of the page to delete.</param>
        public void Delete(String page)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(page, pathBase);
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot delete project files with the delete function.");
            }

            fileFinder.ErasePage(fileInfo.HtmlFile);
        }

        public Task<PageInfoCollection> List(PageQuery query)
        {
            if(query.File != null)
            {
                return Task.FromResult(new PageInfoCollection(query, 1, new PageInfo[] { new PageInfo(query.File) }));
            }
            else
            {
                var fileQuery = fileFinder.EnumerateContentFiles("", "*.html", SearchOption.AllDirectories);
                var total = fileQuery.Count();
                fileQuery = fileQuery.Skip(query.SkipTo(total)).Take(query.Limit);
                return Task.FromResult(new PageInfoCollection(query, total, fileQuery.Select(i => new PageInfo(i))));
            }
        }
    }
}
