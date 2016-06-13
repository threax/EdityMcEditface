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

namespace EdityMcEditface.NetCore.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// This is the location of an additional directory to try to serve files from,
        /// best used to serve the default files this app needs to run.
        /// </summary>
        public static String BackupFileSource = null;

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
                file = detectIndexFile(file);

                this.currentFile = file;
                extension = Path.GetExtension(file).ToLowerInvariant();

                //Fix file name
                sourceFile = file;
                if (extension.Length != 0 && sourceFile.Length > extension.Length)
                {
                    sourceFile = sourceFile.Remove(sourceFile.Length - extension.Length);
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
                        using (var source = new StreamReader(System.IO.File.OpenRead(getRootedPath(sourceFile))))
                        {
                            using (var layout = new StreamReader(System.IO.File.OpenRead(editFile)))
                            {
                                return parsedResponse(source, layout, environment);
                            }
                        }
                    case "":
                        using (var source = new StreamReader(System.IO.File.OpenRead(getRootedPath(sourceFile))))
                        {
                            using (var layout = new StreamReader(System.IO.File.OpenRead("edity/layouts/default.html")))
                            {
                                return parsedResponse(source, layout, environment);
                            }
                        }
                    default:
                        return returnFile(file);
                }
                throw new FileNotFoundException();
            }
            catch (FileNotFoundException)
            {
                //We can get here for a number of reasons, but if the html file does not exist offer to make it
                if (extension == "" && 
                    !System.IO.File.Exists(getRootedPath(sourceFile)) && 
                    !Directory.Exists(getRootedPath(sourceDir)))
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

        private PhysicalFileResult returnFile(String file)
        {
            var content = new FileExtensionContentTypeProvider();
            String contentType;
            if (content.TryGetContentType(file, out contentType))
            {
                if(System.IO.File.Exists(file))
                {
                    return PhysicalFile(Path.GetFullPath(file), contentType);
                }
                //See if the file exists in the backup file location
                if (!String.IsNullOrEmpty(BackupFileSource))
                {
                    var backupFileLoc = Path.Combine(BackupFileSource, file);
                    if (System.IO.File.Exists(backupFileLoc))
                    {
                        return PhysicalFile(Path.GetFullPath(backupFileLoc), contentType);
                    }
                }
            }
            throw new FileNotFoundException($"Cannot find file", file);
        }

        private static string detectIndexFile(string file)
        {
            if (file == null)
            {
                file = "index";
            }
            if (file.Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                file = "index.html";
            }

            return file;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            try
            {
                var file = detectIndexFile(this.Request.Path.ToString().Substring(1));
                if(file == "index")
                {
                    file += ".html";
                }
                String savePath = getRootedPath(file);
                savePath = Path.GetFullPath(savePath);
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

        public ActionResult parsedResponse(TextReader markdown, TextReader template, TemplateEnvironment environment)
        {
            String doc = getConvertedDocument(markdown, template, environment);
            return Content(doc, "text/html");
        }

        public IActionResult Error()
        {
            return View();
        }

        private String getRootedPath(String file)
        {
            return file;
        }
    }
}
