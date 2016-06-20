﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;
using EdityMcEditface.HtmlRenderer;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Net.Http.Headers;

namespace EdityMcEditface.NetCore.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// This is the location of an additional directory to try to serve files from,
        /// best used to serve the default files this app needs to run.
        /// </summary>
        public static String BackupFileSource = null;
        private FileFinder fileFinder;

        public HomeController()
        {

        }

        [HttpGet]
        public IActionResult Index(String file)
        {
            fileFinder = new FileFinder(BackupFileSource);
            fileFinder.useFile(file);

            switch (fileFinder.Extension)
            {
                case ".html":
                    if (fileFinder.IsProjectFile)
                    {
                        return this.PhysicalFile(Path.GetFullPath(fileFinder.SourceFile), "text/html");
                    }
                    return buildAsEditor();
                case "":
                    return buildAsPage();
                default:
                    return returnFile(file);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var file = this.Request.Path.ToString().Substring(1);
            fileFinder = new FileFinder(BackupFileSource);
            fileFinder.useFile(file);

            var savePath = Path.GetFullPath(fileFinder.SourceFile);
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

        public IActionResult Error()
        {
            return View();
        }

        private PhysicalFileResult returnFile(String file)
        {
            var content = new FileExtensionContentTypeProvider();
            String contentType;
            if (content.TryGetContentType(file, out contentType))
            {
                return PhysicalFile(fileFinder.findRealFile(file), contentType);
            }
            throw new FileNotFoundException($"Cannot find file type for '{file}'", file);
        }

        public IActionResult buildAsEditor()
        {
            fileFinder.pushTemplate(fileFinder.getEditorFile("edit"));
            fileFinder.pushTemplate(fileFinder.getLayoutFile("default"));
            fileFinder.pushTemplate(fileFinder.getEditorFile("editarea"));
            return build();
        }

        public IActionResult buildAsPage()
        {
            fileFinder.pushTemplate(fileFinder.getLayoutFile("default"));
            return build();
        }

        public IActionResult build()
        {
            try
            {
                return getConvertedDocument();
            }
            catch (FileNotFoundException)
            {
                //If the source file cannot be read offer to create the new file instead.
                if (fileFinder.PathCanCreateFile)
                {
                    fileFinder.clearTemplates();
                    fileFinder.pushTemplate(fileFinder.getEditorFile("new"));
                    return getConvertedDocument();
                }
                else
                {
                    throw;
                }
            }
        }

        public IActionResult getConvertedDocument()
        {
            DocumentRenderer dr = new DocumentRenderer(fileFinder.Environment);
            var document = dr.getDocument(fileFinder.loadPageStack());
            return Content(document.DocumentNode.OuterHtml, new MediaTypeHeaderValue("text/html"));
        }
    }
}
