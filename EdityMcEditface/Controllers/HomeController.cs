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
        private PageStack pageStack;
        private TargetFileInfo targetFileInfo;
        private TemplateEnvironment templateEnvironment;

        public HomeController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet]
        public IActionResult Index(String file)
        {
            targetFileInfo = new TargetFileInfo(file);
            templateEnvironment = new TemplateEnvironment(targetFileInfo.FileNoExtension, fileFinder.Project);
            PageStack pageStack = new PageStack(templateEnvironment, fileFinder);
            pageStack.ContentFile = targetFileInfo.HtmlFile;

            switch (targetFileInfo.Extension)
            {
                case ".html":
                    if (targetFileInfo.IsProjectFile)
                    {
                        return new FileStreamResult(fileFinder.readFile(targetFileInfo.HtmlFile), "text/html");
                    }
                    return buildAsEditor(pageStack);
                case "":
                    return buildAsPage(pageStack, "default");
                default:
                    var cleanExtension = targetFileInfo.Extension.TrimStart('.');
                    if (fileFinder.doesLayoutExist(cleanExtension))
                    {
                        return buildAsPage(pageStack, cleanExtension);
                    }
                    return returnFile(file);
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        private FileStreamResult returnFile(String file)
        {
            var content = new FileExtensionContentTypeProvider();
            String contentType;
            if (content.TryGetContentType(file, out contentType))
            {
                return new FileStreamResult(fileFinder.readFile(file), contentType);
            }
            throw new FileNotFoundException($"Cannot find file type for '{file}'", file);
        }

        public IActionResult buildAsEditor(PageStack pageStack)
        {
            pageStack.pushLayout("edit");
            pageStack.pushLayout("default");
            pageStack.pushLayout("editarea");
            return build(pageStack);
        }

        public IActionResult buildAsPage(PageStack pageStack, String layout)
        {
            pageStack.pushLayout(layout);
            return build(pageStack);
        }

        public IActionResult build(PageStack pageStack)
        {
            try
            {
                return getConvertedDocument(pageStack);
            }
            catch (FileNotFoundException)
            {
                //If the source file cannot be read offer to create the new file instead.
                if (targetFileInfo.PathCanCreateFile)
                {
                    pageStack = new PageStack(templateEnvironment, fileFinder);
                    pageStack.pushLayout("new");
                    return getConvertedDocument(pageStack);
                }
                else
                {
                    throw;
                }
            }
        }

        public IActionResult getConvertedDocument(PageStack pageStack)
        {
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);
            var document = dr.getDocument(pageStack.Pages);
            return Content(document.DocumentNode.OuterHtml, new MediaTypeHeaderValue("text/html"));
        }
    }
}
