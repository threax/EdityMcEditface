﻿using EdityMcEditface.ErrorHandling;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.CKEditor;
using EdityMcEditface.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]")]
    [Authorize(Roles = Roles.EditPages)]
    public class PageController : Controller
    {
        private FileFinder fileFinder;

        public PageController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet("[action]/{*file}")]
        public PageSettings Settings(String file)
        {
            TargetFileInfo targetFile = new TargetFileInfo(file);
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

        [HttpPost("[action]/{*file}")]
        [AutoValidate("Cannot update page settings.")]
        public void Settings(String file, [FromBody]PageSettings settings)
        {
            TargetFileInfo targetFile = new TargetFileInfo(file);
            var definition = fileFinder.getProjectPageDefinition(targetFile);
            definition.Vars["title"] = settings.Title;
            fileFinder.savePageDefinition(definition, targetFile);
        }

        [HttpPost("{*file}")]
        public async Task<IActionResult> Index(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            var outputFile = fileInfo.OriginalFileName;
            if (fileInfo.Extension == "")
            {
                outputFile = fileInfo.HtmlFile;
            }
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot update project files with the save function.");
            }
            using (Stream stream = fileFinder.writeFile(outputFile))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("{*file}")]
        public IActionResult Delete(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            var outputFile = fileInfo.OriginalFileName;
            if (fileInfo.Extension == "")
            {
                outputFile = fileInfo.HtmlFile;
            }
            if (fileInfo.IsProjectFile)
            {
                throw new ValidationException("Cannot delete project files with the delete function.");
            }

            fileFinder.deleteFile(outputFile);
            fileFinder.deleteFile(fileInfo.FileNoExtension + ".json");
            fileFinder.deleteFile(fileInfo.FileNoExtension + ".css");
            fileFinder.deleteFile(fileInfo.FileNoExtension + ".js");
            fileFinder.deleteFolder(getUploadFolder(fileInfo));

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost("[action]/{*pageUrl}")]
        public async Task<ImageUploadResponse> Asset(String pageUrl)
        {
            ImageUploadResponse imageResponse = new ImageUploadResponse();

            try
            {
                TargetFileInfo fileInfo = new TargetFileInfo(pageUrl);
                string autoFileFolder = getUploadFolder(fileInfo);
                var file = this.Request.Form.Files.First();
                var autoFileFile = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var autoPath = Path.Combine(autoFileFolder, autoFileFile);
                using (Stream stream = fileFinder.writeFile(autoPath))
                {
                    await file.CopyToAsync(stream);
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

        private static string getUploadFolder(TargetFileInfo fileInfo)
        {
            return Path.Combine("AutoUploadedImages", fileInfo.FileNoExtension);
        }
    }
}
