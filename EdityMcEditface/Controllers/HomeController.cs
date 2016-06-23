using System;
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
        private FileFinder fileFinder;

        public HomeController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet]
        public IActionResult Index(String file)
        {
            fileFinder.useFile(file);

            switch (fileFinder.Extension)
            {
                case ".html":
                    if (fileFinder.IsProjectFile)
                    {
                        return new FileStreamResult(fileFinder.readFile(fileFinder.HtmlFile), "text/html");
                    }
                    return buildAsEditor();
                case "":
                    return buildAsPage("default");
                default:
                    var cleanExtension = fileFinder.Extension.TrimStart('.');
                    if (fileFinder.doesLayoutExist(cleanExtension))
                    {
                        return buildAsPage(cleanExtension);
                    }
                    return returnFile(file);
            }
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
            fileFinder.pushLayout(fileFinder.getLayoutFile("edit"));
            fileFinder.pushLayout(fileFinder.getLayoutFile("default"));
            fileFinder.pushLayout(fileFinder.getLayoutFile("editarea"));
            return build();
        }

        public IActionResult buildAsPage(String template)
        {
            fileFinder.pushLayout(fileFinder.getLayoutFile(template));
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
                    fileFinder.clearLayout();
                    fileFinder.SkipHtmlFile = true;
                    fileFinder.pushLayout(fileFinder.getLayoutFile("new"));
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
