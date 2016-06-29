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
using EdityMcEditface.HtmlRenderer.Transforms;

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
                    return buildAsPage(pageStack, "default.html");
                default:
                    var cleanExtension = targetFileInfo.Extension.TrimStart('.') + ".html";
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
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);
            dr.addTransform(new CreateSettingsForm());
            pageStack.pushLayout("edit.html");
            pageStack.pushLayout("default.html");
            pageStack.pushLayout("editarea.html");
            return build(pageStack, dr);
        }

        public IActionResult buildAsPage(PageStack pageStack, String layout)
        {
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);
            pageStack.pushLayout(layout);
            return build(pageStack, dr);
        }

        public IActionResult build(PageStack pageStack, HtmlDocumentRenderer dr)
        {
            try
            {
                return getConvertedDocument(pageStack, dr);
            }
            catch (FileNotFoundException)
            {
                //If the source file cannot be read offer to create the new file instead.
                if (targetFileInfo.PathCanCreateFile)
                {
                    pageStack = new PageStack(templateEnvironment, fileFinder);
                    pageStack.pushLayout("new.html");
                    return getConvertedDocument(pageStack, dr);
                }
                else
                {
                    throw;
                }
            }
        }

        public IActionResult getConvertedDocument(PageStack pageStack, HtmlDocumentRenderer dr)
        {
            var document = dr.getDocument(pageStack.Pages);
            return Content(document.DocumentNode.OuterHtml, new MediaTypeHeaderValue("text/html"));
        }
    }
}
