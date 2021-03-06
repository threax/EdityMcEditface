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
using System.Collections.Concurrent;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Phase;
using EdityMcEditface.Mvc.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EdityMcEditface.Mvc.Controllers
{
    [Authorize(Roles=Roles.EditPages)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(NoStore = true)]
    public class HomeController : Controller
    {
        private IFileFinder fileFinder;
        private ITargetFileInfo targetFileInfo;
        private ITargetFileInfoProvider fileInfoProvider;
        private TemplateEnvironment templateEnvironment;
        private ConcurrentBag<String> seenExtensions = new ConcurrentBag<string>();
        private ConcurrentBag<String> altLayoutExtensions = new ConcurrentBag<string>();
        private IPhaseDetector branchDetector;
        private IOverrideValuesProvider overrideValuesProvider;
        private ILogger<HomeController> logger;

        public HomeController(IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider, IPhaseDetector branchDetector, IOverrideValuesProvider overrideValuesProvider, ILogger<HomeController> logger)
        {
            this.overrideValuesProvider = overrideValuesProvider;
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
            this.branchDetector = branchDetector;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index(String file)
        {
            try
            {
                targetFileInfo = fileInfoProvider.GetFileInfo(file, HttpContext.Request.PathBase);
                PageDefinition pageSettings;

                switch (targetFileInfo.Extension)
                {
                    case ".html":
                        if (targetFileInfo.IsProjectFile)
                        {
                            return new FileStreamResult(fileFinder.ReadFile(targetFileInfo.HtmlFile), "text/html");
                        }
                        return Redirect(targetFileInfo.NoHtmlRedirect);
                    case "":
                        var pageStack = CreatePageStack();
                        pageSettings = fileFinder.GetProjectPageDefinition(targetFileInfo);
                        return buildAsEditor(pageStack, pageSettings.Layout);
                    default:
                        pageSettings = fileFinder.GetProjectPageDefinition(targetFileInfo);
                        var altExt = targetFileInfo.Extension.TrimStart('.') + ".html";
                        var layout = Path.ChangeExtension(pageSettings.Layout, altExt);
                        bool seenBefore = seenExtensions.Contains(layout);
                        if (seenBefore)
                        {
                            //Don't combine these if statements
                            if (altLayoutExtensions.Contains(layout))
                            {
                                return BuildAsAltPage(layout);
                            }
                        }
                        else
                        {
                            seenExtensions.Add(layout);
                            if (fileFinder.DoesLayoutExist(layout))
                            {
                                altLayoutExtensions.Add(layout);
                                return BuildAsAltPage(layout);
                            }
                        }
                        return returnFile(file);
                }
            }
            catch(FileNotFoundException ex)
            {
                var sb = new StringBuilder($"Could not find file '{ex.FileName}' building page for '{file}'. Message:{ex.Message}");
                WriteSearchLocations(sb, ex as PageStackFileNotFoundException);
                logger.LogInformation(sb.ToString());
                throw;
            }
        }

        private IActionResult BuildAsAltPage(string layout)
        {
            PageStack pageStack = CreatePageStack();
            return buildAsPage(pageStack, layout);
        }

        private PageStack CreatePageStack()
        {
            templateEnvironment = new TemplateEnvironment(targetFileInfo.FileNoExtension, fileFinder, overrideVars: overrideValuesProvider.OverrideVars);
            PageStack pageStack = new PageStack(templateEnvironment, fileFinder);
            pageStack.ContentFile = targetFileInfo.HtmlFile;
            return pageStack;
        }

        [Route("edity/preview/{*file}")]
        [HttpGet]
        public IActionResult Preview(String file)
        {
            //This can be optimized like index, but will only be hit
            //for one page if requested, so whatever

            targetFileInfo = fileInfoProvider.GetFileInfo(file, HttpContext.Request.PathBase);
            templateEnvironment = new TemplateEnvironment(targetFileInfo.FileNoExtension, fileFinder, overrideVars: overrideValuesProvider.OverrideVars);
            PageStack pageStack = new PageStack(templateEnvironment, fileFinder);
            pageStack.ContentFile = targetFileInfo.HtmlFile;
            var pageSettings = fileFinder.GetProjectPageDefinition(targetFileInfo);

            switch (targetFileInfo.Extension)
            {
                case ".html":
                    if (targetFileInfo.IsProjectFile)
                    {
                        return new FileStreamResult(fileFinder.ReadFile(targetFileInfo.HtmlFile), "text/html");
                    }
                    return Redirect(targetFileInfo.NoHtmlRedirect);
                case "":
                    return buildAsPage(pageStack, pageSettings.Layout);
                default:
                    var cleanExtension = targetFileInfo.Extension.TrimStart('.') + ".html";
                    if (fileFinder.DoesLayoutExist(cleanExtension))
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
                return new FileStreamResult(fileFinder.ReadFile(file), contentType);
            }
            throw new FileNotFoundException($"Cannot find file type for '{file}'", file);
        }

        private IActionResult buildAsEditor(PageStack pageStack, String layout)
        {
            if (!fileFinder.DoesLayoutExist(layout))
            {
                layout = "default.html"; //Change layout to default.html if the layout does not exist, this way we at least return a page
            }
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);            

            switch (branchDetector.Phase)
            {
                case Phases.Draft:
                    pageStack.pushLayout("draft.html");
                    foreach (var editStackItem in fileFinder.Project.DraftComponents)
                    {
                        pageStack.pushLayout(editStackItem);
                    }

                    pageStack.pushLayout(layout);
                    pageStack.pushLayout("editarea-noedit.html");
                    break;
                case Phases.Edit:
                    pageStack.pushLayout("edit.html");
                    foreach (var editStackItem in fileFinder.Project.EditComponents)
                    {
                        pageStack.pushLayout(editStackItem);
                    }

                    pageStack.pushLayout(layout);
                    pageStack.pushLayout("editarea-ckeditor.html");
                    dr.addTransform(new CreateSettingsForm());
                    dr.addTransform(new CreateTreeMenuEditor());
                    break;
            }

            dr.addTransform(new HashTreeMenus(fileFinder));
            dr.addTransform(new ExpandRootedPaths(this.HttpContext.Request.PathBase));

            return build(pageStack, dr);
        }

        private IActionResult buildAsPage(PageStack pageStack, String layout)
        {
            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(templateEnvironment);
            dr.addTransform(new HashTreeMenus(fileFinder));
            dr.addTransform(new ExpandRootedPaths(this.HttpContext.Request.PathBase));
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
                    return showNewPage(pageStack, dr);
                }
                else
                {
                    throw;
                }
            }
            catch (FileNotFoundException ex)
            {
                //If the source file cannot be read offer to create the new file instead.
                if (targetFileInfo.PathCanCreateFile)
                {
                    var sb = new StringBuilder($"Could not find file '{ex.FileName}' Message: '{ex.Message}' building page '{pageStack.ContentFile}'. Showing new page.");
                    WriteSearchLocations(sb, ex as PageStackFileNotFoundException);
                    logger.LogInformation(sb.ToString());
                    return showNewPage(pageStack, dr);
                }
                else
                {
                    throw;
                }
            }
        }

        private static void WriteSearchLocations(StringBuilder sb, PageStackFileNotFoundException pageStackEx)
        {
            if (pageStackEx != null)
            {
                sb.AppendLine(" Searched:");
                foreach (var item in pageStackEx.SearchLocations)
                {
                    sb.AppendLine(item);
                }
            }
        }

        private IActionResult showNewPage(PageStack pageStack, HtmlDocumentRenderer dr)
        {
            switch (branchDetector.Phase)
            {
                case Phases.Draft:
                    pageStack.pushLayout("notfound.html");
                    pageStack.ContentFile = null;
                    break;
                case Phases.Edit:
                    pageStack = new PageStack(templateEnvironment, fileFinder);
                    pageStack.pushLayout("new.html");
                    break;
            }
            return getConvertedDocument(pageStack, dr);
        }

        private IActionResult getConvertedDocument(PageStack pageStack, HtmlDocumentRenderer dr)
        {
            var document = dr.getDocument(pageStack.Pages);
            return Content(document.DocumentNode.OuterHtml, new MediaTypeHeaderValue("text/html"));
        }
    }
}
