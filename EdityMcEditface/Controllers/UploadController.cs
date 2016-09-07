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
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EdityMcEditface.Controllers
{
    [Authorize(Roles = Roles.UploadAnything)]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            using (Stream stream = fileFinder.writeFile(fileInfo.DerivedFileName))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("edity/upload/{*file}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            if (fileInfo.PointsToHtmlFile)
            {
                fileFinder.erasePage(fileInfo.HtmlFile);
            }
            else
            {
                fileFinder.eraseProjectFile(fileInfo.DerivedFileName);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}
