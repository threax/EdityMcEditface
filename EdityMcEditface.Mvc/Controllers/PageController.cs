﻿using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.CKEditor;
using EdityMcEditface.Mvc.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This controller handles properties of a page.
    /// </summary>
    [Route("edity/[controller]/[action]")]
    [Authorize(Roles = Roles.EditPages)]
    [ResponseCache(NoStore = true)]
    public class PageController : Controller
    {
        private IFileFinder fileFinder;
        private ITargetFileInfoProvider fileInfoProvider;

        public PageController(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
        }

        /// <summary>
        /// Get the current page settings.
        /// </summary>
        /// <param name="page">The name of the file to lookup.</param>
        /// <returns>The PageSettings for the file.</returns>
        [HttpGet]
        public PageSettings GetSettings([FromQuery] String page)
        {
            var targetFile = fileInfoProvider.GetFileInfo(page, HttpContext.Request.PathBase);
            var definition = fileFinder.GetProjectPageDefinition(targetFile);
            String title;
            if(!definition.Vars.TryGetValue("title", out title))
            {
                title = "Untitled";
            }
            return new PageSettings()
            {
                Title = title,
                Visible = !definition.Hidden
            };
        }

        /// <summary>
        /// Update the settings for the page.
        /// </summary>
        /// <param name="page">The file who's pages to upload.</param>
        /// <param name="settings">The page settings to set.</param>
        [HttpPut]
        [AutoValidate("Cannot update page settings.")]
        public void UpdateSettings([FromQuery] String page, [FromBody]PageSettings settings)
        {
            var targetFile = fileInfoProvider.GetFileInfo(page, HttpContext.Request.PathBase);
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
        [HttpPut]
        public async Task Save([FromQuery] String page, IFormFile content)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(page, HttpContext.Request.PathBase);
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot update project files with the save function.");
            }
            using (Stream stream = fileFinder.WriteFile(fileInfo.DerivedFileName))
            {
                await content.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Delete a page.
        /// </summary>
        /// <param name="page">The name of the page to delete.</param>
        [HttpDelete]
        public void Delete([FromQuery] String page)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(page, HttpContext.Request.PathBase);
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot delete project files with the delete function.");
            }

            fileFinder.ErasePage(fileInfo.HtmlFile);
        }

        /// <summary>
        /// Add an asset to a page.
        /// </summary>
        /// <param name="page">The page to add the asset to.</param>
        /// <param name="upload">The file content.</param>
        /// <returns>The ImageUplaodResponse with the result.</returns>
        [HttpPost]
        public async Task<ImageUploadResponse> AddAsset([FromQuery] String page, IFormFile upload)
        {
            ImageUploadResponse imageResponse = new ImageUploadResponse();

            try
            {
                var fileInfo = fileInfoProvider.GetFileInfo(page, HttpContext.Request.PathBase);
                string autoFileFolder = "AutoUploads";
                var autoFileFile = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                var autoPath = Path.Combine(autoFileFolder, autoFileFile);
                using (Stream stream = fileFinder.WriteFile(autoPath))
                {
                    await upload.CopyToAsync(stream);
                }

                imageResponse.Uploaded = 1;
                imageResponse.FileName = autoFileFile;
                imageResponse.Url = HttpContext.Request.PathBase + autoPath.EnsureStartingPathSlash();
            }
            catch (Exception ex)
            {
                imageResponse.Message = ex.Message;
                imageResponse.Uploaded = 0;
            }

            return imageResponse;
        }
    }
}
