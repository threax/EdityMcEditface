using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.CKEditor;
using EdityMcEditface.Models.Page;
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

namespace EdityMcEditface.Controllers
{
    /// <summary>
    /// This controller handles properties of a page.
    /// </summary>
    [Route("edity/[controller]/[action]")]
    [Authorize(Roles = Roles.EditPages)]
    public class PageController : Controller
    {
        private FileFinder fileFinder;

        public PageController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// Get the current page settings.
        /// </summary>
        /// <param name="page">The name of the file to lookup.</param>
        /// <returns>The PageSettings for the file.</returns>
        [HttpGet]
        public PageSettings GetSettings([FromQuery] String page)
        {
            TargetFileInfo targetFile = new TargetFileInfo(page);
            var definition = fileFinder.getProjectPageDefinition(targetFile);
            String title;
            if(!definition.Vars.TryGetValue("title", out title))
            {
                title = "Untitled";
            }
            return new PageSettings()
            {
                Title = title
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
            TargetFileInfo targetFile = new TargetFileInfo(page);
            var definition = fileFinder.getProjectPageDefinition(targetFile);
            definition.Vars["title"] = settings.Title;
            fileFinder.savePageDefinition(definition, targetFile);
        }

        /// <summary>
        /// Save a page.
        /// </summary>
        /// <param name="page">The file to save.</param>
        /// <param name="content">The file content.</param>
        [HttpPut]
        public async Task Save([FromQuery] String page, IFormFile content)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(page);
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot update project files with the save function.");
            }
            using (Stream stream = fileFinder.writeFile(fileInfo.DerivedFileName))
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
            TargetFileInfo fileInfo = new TargetFileInfo(page);
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot delete project files with the delete function.");
            }

            fileFinder.deleteFile(fileInfo.DerivedFileName);
            fileFinder.deleteFile(fileInfo.FileNoExtension + ".json");
            fileFinder.deleteFile(fileInfo.FileNoExtension + ".css");
            fileFinder.deleteFile(fileInfo.FileNoExtension + ".js");
            fileFinder.deleteFolder(getUploadFolder(fileInfo));
        }

        /// <summary>
        /// Add an asset to a page.
        /// </summary>
        /// <param name="page">The page to add the asset to.</param>
        /// <param name="content">The file content.</param>
        /// <returns>The ImageUplaodResponse with the result.</returns>
        [HttpPost]
        public async Task<ImageUploadResponse> AddAsset([FromQuery] String page, IFormFile content)
        {
            ImageUploadResponse imageResponse = new ImageUploadResponse();

            try
            {
                TargetFileInfo fileInfo = new TargetFileInfo(page);
                string autoFileFolder = getUploadFolder(fileInfo);
                var autoFileFile = Guid.NewGuid().ToString() + Path.GetExtension(content.FileName);
                var autoPath = Path.Combine(autoFileFolder, autoFileFile);
                using (Stream stream = fileFinder.writeFile(autoPath))
                {
                    await content.CopyToAsync(stream);
                }

                imageResponse.Uploaded = 1;
                imageResponse.FileName = autoFileFile;
                if (autoPath[0] != '\\' && autoPath[0] != '/')
                {
                    autoPath = '/' + autoPath;
                }
                imageResponse.Url = autoPath;
            }
            catch (Exception ex)
            {
                imageResponse.Message = ex.Message;
                imageResponse.Uploaded = 0;
            }

            return imageResponse;
        }

        /// <summary>
        /// Helper funciton to get the upload folder.
        /// </summary>
        /// <param name="fileInfo">The file info to use.</param>
        /// <returns>The upload folder for the given file.</returns>
        private static string getUploadFolder(TargetFileInfo fileInfo)
        {
            return Path.Combine("AutoUploadedImages", fileInfo.FileNoExtension);
        }
    }
}
