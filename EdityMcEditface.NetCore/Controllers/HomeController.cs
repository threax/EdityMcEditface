using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;
using Edity.McEditface.HtmlRenderer;
using System.Net;

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
        private String tempViewPath;
        private TemplateEnvironment environment;

        public HomeController()
        {

        }

        public void Dispose()
        {
            if(tempViewPath != null)
            {
                Console.WriteLine(tempViewPath);
            }
        }

        [HttpGet]
        public IActionResult Get(String file)
        {
            try
            {
                if(file == null)
                {
                    file = "";
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

                using (var source = new StreamReader(System.IO.File.OpenRead(sourceFile)))
                {
                    switch (extension)
                    {
                        case ".edit":
                        case ".html":
                            var editFile = "edity/editor/edit.html";
                            if (sourceFile.StartsWith("edity/templates", StringComparison.OrdinalIgnoreCase))
                            {
                                return this.PhysicalFile(sourceFile, "text/html");
                            }
                            if (System.IO.File.Exists(editFile))
                            {
                                using (var layout = new StreamReader(System.IO.File.OpenRead(editFile)))
                                {
                                    return parsedResponse(source, layout, environment);
                                }
                            }
                            else
                            {
                                throw new FileNotFoundException();
                                //using (var layout = new StreamReader(assembly.GetManifestResourceStream("Edity.McEditface.BackendTemplates.edit.html")))
                                //{
                                //    return parsedResponse(source, layout, environment);
                                //}
                            }
                        case "":
                            using (var layout = new StreamReader(System.IO.File.OpenRead("edity/layouts/default.html")))
                            {
                                return parsedResponse(source, layout, environment);
                            }
                        default:
                            throw new FileNotFoundException();
                    }
                }
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

        //[HttpPost]
        //public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        //{
        //    try
        //    {
        //        MultipartMemoryStreamProvider x = await request.Content.ReadAsMultipartAsync();
        //        String file = request.RequestUri.AbsolutePath.Remove(0, UploadPath.Length + 1); //Starts with a slash where our defined path does not so + 1
        //        String directory = Path.GetDirectoryName(file);
        //        if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        //        {
        //            Directory.CreateDirectory(directory);
        //        }
        //        using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
        //        {
        //            await x.Contents[0].CopyToAsync(stream);
        //        }
        //        return this.statusCodeResponse(System.Net.HttpStatusCode.OK);
        //    }
        //    catch (Exception)
        //    {
        //        return this.statusCodeResponse(System.Net.HttpStatusCode.InternalServerError);
        //    }
        //}
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
            tempViewPath = $"Views/Temp/{Guid.NewGuid().ToString()}.cshtml";
            using (var stream = new StreamWriter(System.IO.File.Open(tempViewPath, FileMode.Create)))
            {
                stream.Write(doc);
            }
            return View(tempViewPath);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
