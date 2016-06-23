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

        //[HttpGet("edity/list/{*file}")]
        //public IActionResult ListFiles(String file)
        //{
        //    fileFinder.useFile(file);
        //    
        //    var fullDirectoryPath = fileFinder.getFullRealPath(fileFinder.DirectoryPath);
        //    if ((Directory.Exists(fullDirectoryPath) 
        //        && !System.IO.File.Exists(fileFinder.getFullRealPath(fileFinder.HtmlFile))))
        //    {
        //        return Json(new
        //        {
        //            directories = Directory.EnumerateDirectories(fullDirectoryPath, "*", SearchOption.TopDirectoryOnly).Where(f => !System.IO.File.Exists(f + ".html")),
        //            files = Directory.EnumerateFiles(fullDirectoryPath, "*", SearchOption.TopDirectoryOnly)
        //        });
        //    }
        //    return StatusCode((int)HttpStatusCode.OK);
        //}

        [HttpPost("edity/upload/{*file}")]
        public async Task<IActionResult> Index(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            using (Stream stream = fileFinder.writeFile(fileInfo.HtmlFile))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}
