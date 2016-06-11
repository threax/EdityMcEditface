using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;
using Edity.McEditface.HtmlRenderer;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;

namespace EdityMcEditface.NetCore.Controllers
{
    public class HomeController : Controller
    {
        public const String UploadPath = "ide/api/upload/";
        public const String DirApiPath = "ide/api/dir/";

        private static char[] seps = { '|' };

        private String currentFile;
        private String sourceFile;
        private String sourceDir;
        private String extension;
        private TemplateEnvironment environment;

        public HomeController()
        {

        }

        [HttpGet]
        public IActionResult Index(String file)
        {
            try
            {
                if(file == null)
                {
                    file = "index";
                }
                if(file.Equals(".html", StringComparison.OrdinalIgnoreCase))
                {
                    file = "index.html";
                }

                this.currentFile = file;
                extension = Path.GetExtension(file).ToLowerInvariant();

                //Fix file name
                sourceFile = file;
                if (extension.Length != 0 && sourceFile.Length > extension.Length)
                {
                    sourceFile = sourceFile.Remove(sourceFile.Length - extension.Length);
                }

                if (sourceFile.StartsWith(HomeController.DirApiPath))
                {
                    sourceFile = sourceFile.Substring(HomeController.DirApiPath.Length);
                }
                sourceDir = sourceFile;
                environment = new TemplateEnvironment("/" + sourceFile);
                sourceFile = sourceFile + ".html";

                if (string.IsNullOrEmpty(extension))
                {
                    if (String.IsNullOrEmpty(sourceDir))
                    {
                        sourceDir = ".";
                    }

                    if ((Directory.Exists(sourceDir) && !System.IO.File.Exists(sourceFile)))
                    {
                        return Json(new
                        {
                            directories = Directory.EnumerateDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly).Where(f => !System.IO.File.Exists(f + ".html")),
                            files = Directory.EnumerateFiles(sourceDir, "*", SearchOption.TopDirectoryOnly)
                        });
                    }
                }

                
                switch (extension)
                {
                    case ".html":
                        var editFile = "edity/editor/edit.html";
                        if (sourceFile.StartsWith("edity/templates", StringComparison.OrdinalIgnoreCase))
                        {
                            return this.PhysicalFile(Path.GetFullPath(sourceFile), "text/html");
                        }
                        using (var source = new StreamReader(System.IO.File.OpenRead(sourceFile)))
                        {
                            using (var layout = new StreamReader(System.IO.File.OpenRead(editFile)))
                            {
                                return parsedResponse(source, layout, environment);
                            }
                        }
                    case "":
                        using (var source = new StreamReader(System.IO.File.OpenRead(sourceFile)))
                        {
                            using (var layout = new StreamReader(System.IO.File.OpenRead("edity/layouts/default.html")))
                            {
                                return parsedResponse(source, layout, environment);
                            }
                        }
                    default:
                        var content = new FileExtensionContentTypeProvider();
                        String contentType;
                        if(content.TryGetContentType(file, out contentType))
                        {
                            return PhysicalFile(Path.GetFullPath(file), contentType);
                        }
                        break;
                }
                throw new FileNotFoundException();
            }
            catch (FileNotFoundException)
            {
                //We can get here for a number of reasons, but if the html file does not exist offer to make it
                if (!System.IO.File.Exists(sourceFile) && extension == "")
                {
                    try
                    {
                        String newLayout = "edity/editor/new.html";
                        if (System.IO.File.Exists(newLayout))
                        {
                            using (var source = new StringReader(""))
                            {
                                using (var layout = new StreamReader(System.IO.File.OpenRead(newLayout)))
                                {
                                    return parsedResponse(source, layout, environment);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {
            try
            {
                //Starts with a slash where our defined path does not so + 1
                String savePath = this.Request.Path.ToString().Remove(0, UploadPath.Length + 1);
                savePath = Path.GetFullPath(savePath);
                String directory = Path.GetDirectoryName(savePath);
                if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (Stream stream = System.IO.File.Open(savePath, FileMode.Create, FileAccess.Write))
                {
                    await files.First().CopyToAsync(stream);
                }
                return StatusCode((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //data-settings-form
        public String getConvertedDocument(TextReader markdown, TextReader template, TemplateEnvironment environment)
        {
            DocumentRenderer dr = new DocumentRenderer(template.ReadToEnd(), environment);
            return dr.getDocument(markdown.ReadToEnd());
        }

        private const String TempViewPath = "Views/Temp/";

        public ActionResult parsedResponse(TextReader markdown, TextReader template, TemplateEnvironment environment)
        {
            String doc = getConvertedDocument(markdown, template, environment);
            return Content(doc, "text/html");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
