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
    /// <summary>
    /// This controller handles file uploads.
    /// </summary>
    [Authorize(Roles = Roles.UploadAnything)]
    [Route("edity/[controller]/[action]")]
    public class UploadController : Controller
    {
        private FileFinder fileFinder;

        /// <summary>
        /// Controller.
        /// </summary>
        /// <param name="fileFinder"></param>
        public UploadController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// List the files in dir.
        /// </summary>
        /// <param name="dir">The directory to list the files under.</param>
        /// <returns>A list of files under dir.</returns>
        [HttpGet("{*file}")]
        public IActionResult ListFiles(String dir)
        {
            if(dir == null)
            {
                dir = "";
            }

            return Json(new
            {
                directories = fileFinder.enumerateDirectories(dir),
                files = fileFinder.enumerateFiles(dir)
            });
        }

        /// <summary>
        /// Uplaod a new file.
        /// </summary>
        /// <param name="file">The file name of the uploaded file.</param>
        /// <returns></returns>
        [HttpPost("{*file}")]
        public async Task<IActionResult> Upload(String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            using (Stream stream = fileFinder.writeFile(fileInfo.DerivedFileName))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <returns></returns>
        [HttpDelete("{*file}")]
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
