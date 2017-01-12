using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdityMcEditface.HtmlRenderer;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using EdityMcEditface.Mvc.Models.CKEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using EdityMcEditface.Mvc.Models.Upload;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This controller handles file uploads.
    /// </summary>
    [Authorize(Roles = Roles.UploadAnything)]
    [Route("edity/[controller]/[action]")]
    public class UploadController : Controller
    {
        private IFileFinder fileFinder;

        /// <summary>
        /// Controller.
        /// </summary>
        /// <param name="fileFinder"></param>
        public UploadController(IFileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// List the files in dir.
        /// </summary>
        /// <param name="dir">The directory to list the files under.</param>
        /// <returns>A list of files under dir.</returns>
        [HttpGet]
        public FileList ListFiles([FromQuery] String dir)
        {
            if(dir == null)
            {
                dir = "";
            }

            return new FileList
            {
                Directories = fileFinder.EnumerateContentDirectories(dir),
                Files = fileFinder.EnumerateContentFiles(dir),
                Path = dir
            };
        }

        /// <summary>
        /// Uplaod a new file.
        /// </summary>
        /// <param name="file">The file name of the uploaded file.</param>
        /// <param name="content">The file content.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task Upload([FromQuery] String file, IFormFile content)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            using (Stream stream = fileFinder.WriteFile(fileInfo.DerivedFileName))
            {
                await content.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        public void Delete([FromQuery] String file)
        {
            TargetFileInfo fileInfo = new TargetFileInfo(file);
            if (fileInfo.PointsToHtmlFile)
            {
                fileFinder.ErasePage(fileInfo.HtmlFile);
            }
            else
            {
                fileFinder.DeleteFile(fileInfo.DerivedFileName);
            }
        }
    }
}
