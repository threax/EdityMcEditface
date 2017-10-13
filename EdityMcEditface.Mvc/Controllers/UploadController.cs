using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdityMcEditface.HtmlRenderer;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using EdityMcEditface.Mvc.Models.Upload;
using EdityMcEditface.HtmlRenderer.FileInfo;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.FileRepository;

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This controller handles file uploads.
    /// </summary>
    [Authorize(Roles = Roles.UploadAnything)]
    [Route("edity/[controller]/[action]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    public class UploadController : Controller
    {
        public static class Rels
        {
            public const String UploadFile = "UploadFile";
            public const String ListUploadedFiles = "ListUploadedFiles";
            public const String DeleteFile = "DeleteFile";
        }

        private IFileFinder fileFinder;
        private ITargetFileInfoProvider fileInfoProvider;

        /// <summary>
        /// Controller.
        /// </summary>
        /// <param name="fileFinder"></param>
        /// <param name="fileInfoProvider">The file info provider.</param>
        public UploadController(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
        }

        /// <summary>
        /// List the files in dir.
        /// </summary>
        /// <param name="query">The query with the directory to list the files under.</param>
        /// <returns>A list of files under dir.</returns>
        [HttpGet]
        [HalRel(Rels.ListUploadedFiles)]
        public FileList List([FromQuery] ListFileQuery query)
        {
            var dir = query.Dir;

            if(dir == null)
            {
                dir = "";
            }

            dir = DefaultTargetFileInfo.RemovePathBase(dir, HttpContext.Request.PathBase);

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
        /// <param name="input">The upload input data.</param>
        /// <param name="fileVerifier">The file verifier.</param>
        /// <returns></returns>
        [HttpPost]
        [HalRel(Rels.UploadFile)]
        public async Task Upload([FromForm] UploadInput input, [FromServices] IFileVerifier fileVerifier)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(input.File, HttpContext.Request.PathBase);
            using (var uploadStream = input.Content.OpenReadStream())
            {
                fileVerifier.Validate(uploadStream, fileInfo.DerivedFileName, input.Content.ContentType);
                using (Stream stream = fileFinder.WriteFile(fileInfo.DerivedFileName))
                {
                    await uploadStream.CopyToAsync(stream);
                }
            }
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpDelete]
        [HalRel(Rels.DeleteFile)]
        public void Delete([FromQuery] DeleteFileQuery query)
        {
            var fileInfo = fileInfoProvider.GetFileInfo(query.File, HttpContext.Request.PathBase);
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
