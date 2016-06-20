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
            if ((Directory.Exists(fileFinder.DirectoryPath) && !System.IO.File.Exists(fileFinder.HtmlFile)))
            {
                return Json(new
                {
                    directories = Directory.EnumerateDirectories(fileFinder.DirectoryPath, "*", SearchOption.TopDirectoryOnly).Where(f => !System.IO.File.Exists(f + ".html")),
                    files = Directory.EnumerateFiles(fileFinder.DirectoryPath, "*", SearchOption.TopDirectoryOnly)
                });
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost("edity/upload/{*file}")]
        public async Task<IActionResult> Index(String file)
        {
            fileFinder.useFile(file);

            var savePath = fileFinder.getFullRealPath(fileFinder.HtmlFile);
            String directory = Path.GetDirectoryName(savePath);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (Stream stream = System.IO.File.Open(savePath, FileMode.Create, FileAccess.Write))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}
