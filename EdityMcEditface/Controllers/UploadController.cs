using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdityMcEditface.HtmlRenderer;
using System.IO;
using System.Net;

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
