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
using EdityMcEditface.HtmlRenderer.Transforms;
using Microsoft.AspNetCore.Authorization;

namespace EdityMcEditface.NetCore.Controllers
{
    [Authorize(Roles=Roles.EditPages)]
    //[Authorize]
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
                    return Redirect(targetFileInfo.NoHtmlRedirect);
                case "":
                    return buildAsEditor(pageStack);
                default:
                    var cleanExtension = targetFileInfo.Extension.TrimStart('.') + ".html";
                    if (fileFinder.doesLayoutExist(cleanExtension))
                    {
                        return buildAsPage(pageStack, cleanExtension);
                    }
                    return returnFile(file);
            }
        }

        [Route("edity/preview/{*file}")]
        [HttpGet]
        public IActionResult Preview(String file)
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
                    return Redirect(targetFileInfo.NoHtmlRedirect);
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

        private IActionResult buildAsEditor(PageStack pageStack)
        {
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);
            dr.addTransform(new CreateSettingsForm());
            dr.addTransform(new CreateTreeMenuEditor());
            pageStack.pushLayout("edit.html");
            pageStack.pushLayout("default.html");
            foreach (var editStackItem in fileFinder.Project.EditComponents)
            {
                pageStack.pushLayout(editStackItem);
            }
            pageStack.pushLayout("editarea-ckeditor.html");

            return build(pageStack, dr);
        }

        private IActionResult buildAsPage(PageStack pageStack, String layout)
        {
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);
            pageStack.pushLayout(layout);
            return build(pageStack, dr);
        }

        private IActionResult build(PageStack pageStack, HtmlDocumentRenderer dr)
        {
            try
            {
                return getConvertedDocument(pageStack, dr);
            }
            catch (DirectoryNotFoundException)
            {
                //If the source file cannot be read offer to create the new file instead.
                if (targetFileInfo.PathCanCreateFile)
                {
                    return showNewPage(dr);
                }
                else
                {
                    throw;
                }
            }
            catch (FileNotFoundException)
            {
                //If the source file cannot be read offer to create the new file instead.
                if (targetFileInfo.PathCanCreateFile)
                {
                    return showNewPage(dr);
                }
                else
                {
                    throw;
                }
            }
        }

        private IActionResult showNewPage(HtmlDocumentRenderer dr)
        {
            pageStack = new PageStack(templateEnvironment, fileFinder);
            pageStack.pushLayout("new.html");
            return getConvertedDocument(pageStack, dr);
        }

        private IActionResult getConvertedDocument(PageStack pageStack, HtmlDocumentRenderer dr)
        {
            var document = dr.getDocument(pageStack.Pages);
            return Content(document.DocumentNode.OuterHtml, new MediaTypeHeaderValue("text/html"));
        }
    }
}
