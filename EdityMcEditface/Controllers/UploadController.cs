using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdityMcEditface.HtmlRenderer;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using EdityMcEditface.Models.CKEditor;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EdityMcEditface.Controllers
{
    public class UploadController : Controller
    {
        private FileFinder fileFinder;

        public UploadController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet("edity/list/{*file}")]
        public IActionResult ListFiles(String file)
        {
            if(file == null)
            {
                file = "";
            }

            return Json(new
            {
                directories = fileFinder.enumerateDirectories(file),
                files = fileFinder.enumerateFiles(file)
            });
        }

        [HttpPost("edity/upload/{*file}")]
        public async Task<IActionResult> Index(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            using (Stream stream = fileFinder.writeFile(fileInfo.OriginalFileName))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost("edity/upload/pageasset/{*pageUrl}")]
        public async Task<ImageUploadResponse> UploadPageAsset(String pageUrl)
        {
            ImageUploadResponse imageResponse = new ImageUploadResponse();

            try
            {
                TargetFileInfo fileInfo = new TargetFileInfo(pageUrl);
                var autoFileFolder = Path.Combine("AutoUploadedImages", fileInfo.FileNoExtension);
                var file = this.Request.Form.Files.First();
                var autoFileFile = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var autoPath = Path.Combine(autoFileFolder, autoFileFile);
                using (Stream stream = fileFinder.writeFile(autoPath))
                {
                    await file.CopyToAsync(stream);
                }

                imageResponse.Uploaded = 1;
                imageResponse.FileName = autoFileFile;
                if(autoPath[0] != '\\' && autoPath[0] != '/')
                {
                    autoPath = '/' + autoPath;
                }
                imageResponse.Url = autoPath;
            }
            catch(Exception ex)
            {
                imageResponse.Message = ex.Message;
                imageResponse.Uploaded = 0;
            }

            return imageResponse;
        }

        [HttpDelete("edity/upload/{*file}")]
        public IActionResult Delete(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            if (fileInfo.Extension == ".html")
            {
                fileFinder.erasePage(fileInfo.HtmlFile);
            }
            else
            {
                fileFinder.eraseProjectFile(fileInfo.OriginalFileName);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}
