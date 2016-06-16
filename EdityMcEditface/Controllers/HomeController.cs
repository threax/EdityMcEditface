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

                String projectStr = "";
                using (var reader = new StreamReader(System.IO.File.Open(findRealFile("edity/edity.json"), FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    projectStr = reader.ReadToEnd();
                }

                var project = JsonConvert.DeserializeObject<EdityProject>(projectStr);

                sourceDir = sourceFile;
                environment = new TemplateEnvironment("/" + sourceFile, project);
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

                List<String> templates = new List<string>();
                switch (extension)
                {
                    case ".html":
                        if (sourceFile.StartsWith("edity/", StringComparison.OrdinalIgnoreCase))
                        {
                            return this.PhysicalFile(Path.GetFullPath(sourceFile), "text/html");
                        }
                        templates.Add(getLayoutFile("default"));
                        templates.Add(getEditorFile("edit"));
                        break;
                    case "":
                        templates.Add(getLayoutFile("default"));
                        break;
                    default:
                        return returnFile(file);
                }

                using (var source = new StreamReader(System.IO.File.OpenRead(sourceFile)))
                {
                    return parsedResponse(source, templates, environment);
                }
            }
            catch (FileNotFoundException)
            {
                //We can get here for a number of reasons, but if the html file does not exist offer to make it
                if (extension == "" && 
                    !System.IO.File.Exists(sourceFile) && 
                    !Directory.Exists(sourceDir))
                {
                    try
                    {
                        String newLayout = getEditorFile("new");
                        if (System.IO.File.Exists(newLayout))
                        {
                            using (var source = new StringReader(""))
                            {
                                return parsedResponse(source, new String[] { newLayout }, environment);
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
                var savePath = Path.GetFullPath(file);
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
        public String getConvertedDocument(TextReader content, IEnumerable<String> templates, TemplateEnvironment environment)
        {
            DocumentRenderer dr = new DocumentRenderer(environment);

            foreach (var template in templates)
            {
                var realTemplate = findRealFile(template);
                using (var layout = new StreamReader(System.IO.File.OpenRead(realTemplate)))
                {
                    dr.pushTemplate(new PageStackItem()
                    {
                        Content = layout.ReadToEnd(),
                        PageDefinition = getPageDefinition(realTemplate),
                        PageScriptPath = getPageFile(realTemplate, template, "js"),
                        PageCssPath = getPageFile(realTemplate, template, "css"),
                    });
                }
            }
            var document = dr.getDocument(new PageStackItem()
            {
                Content = content.ReadToEnd(),
                PageDefinition = getPageDefinition(sourceFile),
                PageScriptPath = getPageFile(sourceFile, sourceFile, "js"),
                PageCssPath = getPageFile(sourceFile, sourceFile, "css"),
            });
            
            return document.DocumentNode.OuterHtml;
        }

        public ActionResult parsedResponse(TextReader content, IEnumerable<String> templates, TemplateEnvironment environment)
        {
            String doc = getConvertedDocument(content, templates, environment);
            return Content(doc, "text/html");
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
                return PhysicalFile(findRealFile(file), contentType);
            }
            throw new FileNotFoundException($"Cannot find file type for '{file}'", file);
        }

        /// <summary>
        /// Find the full real file path if the file exists, if not returns the original
        /// file name.
        /// </summary>
        /// <param name="file">The file to look for.</param>
        /// <returns>The full path to the real file, searching all dirs.</returns>
        private static String findRealFile(String file)
        {
            if (System.IO.File.Exists(file))
            {
                return Path.GetFullPath(file);
            }

            var backupFileLoc = Path.Combine(BackupFileSource, file);
            if (System.IO.File.Exists(backupFileLoc))
            {
                return Path.GetFullPath(backupFileLoc);
            }

            //Not found, just return the file
            return file;
        }

        private static string getEditorFile(String layoutName)
        {
            //returnFile
            String file = $"edity/editor/{layoutName}.html";
            return file;
        }

        private static string getLayoutFile(String layoutName)
        {
            //returnFile
            String file = $"edity/layouts/{layoutName}.html";
            return file;
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

        private PageDefinition getPageDefinition(String file)
        {
            String settingsPath = Path.ChangeExtension(file, "json");
            PageDefinition pageSettings;
            if (System.IO.File.Exists(settingsPath))
            {
                using (var stream = new StreamReader(System.IO.File.Open(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    pageSettings = JsonConvert.DeserializeObject<PageDefinition>(stream.ReadToEnd());
                }
            }
            else
            {
                pageSettings = new PageDefinition();
            }

            return pageSettings;
        }

        /// <summary>
        /// Get the page file with the given extension that exists in the same folder as hostFile,
        /// the linkFile will be returned with the correct path so you can easily stay in web
        /// server instead of file system scope.
        /// </summary>
        /// <param name="hostFile">The file to check for a matching file, the extension will be replaced.</param>
        /// <param name="linkFile">The file to use to link to the hostFile in the webserver context.</param>
        /// <param name="extension">The extension of the file to look for.</param>
        /// <returns>The matching linkFile name or null if it is not found.</returns>
        private String getPageFile(String hostFile, String linkFile, String extension)
        {
            String realPath = Path.ChangeExtension(hostFile, extension);
            if (System.IO.File.Exists(realPath))
            {
                return Path.ChangeExtension(linkFile, extension);
            }
            return null;
        }
    }
}
