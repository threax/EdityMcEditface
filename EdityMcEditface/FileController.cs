using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Edity.McEditface.HtmlRenderer;
using System.Threading.Tasks;
using System.Linq;

namespace Edity.McEditface
{
    public class FileController : ApiController
    {
        public const String UploadPath = "ide/api/upload/";
        public const String DirApiPath = "ide/api/dir/";

        private static char[] seps = { '|' };
        private static Assembly assembly = Assembly.GetExecutingAssembly();

        private String currentFile;

        [HttpGet]
        public HttpResponseMessage Get(String file)
        {
            try
            {
                this.currentFile = file;
                var extension = Path.GetExtension(file).ToLowerInvariant();

                //Fix file name
                var sourceFile = file;
                if (extension.Length != 0 && sourceFile.Length > extension.Length)
                {
                    sourceFile = sourceFile.Remove(sourceFile.Length - extension.Length);
                }
                
                if(sourceFile.StartsWith(FileController.DirApiPath))
                {
                    sourceFile = sourceFile.Substring(FileController.DirApiPath.Length);
                }
                var sourceDir = sourceFile;
                sourceFile = sourceFile + ".html";

                if (string.IsNullOrEmpty(extension))
                {
                    if (String.IsNullOrEmpty(sourceDir))
                    {
                        sourceDir = ".";
                    }

                    if ((Directory.Exists(sourceDir) && !File.Exists(sourceFile)))
                    {
                        return this.jsonResponse(new
                        {
                            directories = Directory.EnumerateDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly).Where(f => !File.Exists(f + ".html")),
                            files = Directory.EnumerateFiles(sourceDir, "*", SearchOption.TopDirectoryOnly)
                        });
                    }
                }

                TemplateEnvironment environment = new TemplateEnvironment("/" + file);
                using (var source = new StreamReader(File.OpenRead(sourceFile)))
                {
                    switch (extension)
                    {
                        case ".edit":
                        case ".html":
                            if (File.Exists("edity/layouts/edit.html"))
                            {
                                using (var template = new StreamReader(File.OpenRead("edity/layouts/edit.html")))
                                {
                                    return parsedResponse(source, template, environment);
                                }
                            }
                            else
                            {
                                using (var template = new StreamReader(assembly.GetManifestResourceStream("Edity.McEditface.BackendTemplates.edit.html")))
                                {
                                    return parsedResponse(source, template, environment);
                                }
                            }
                        case ".text":
                            return viewMarkdownResponse(source);
                        case "":
                            using (var template = new StreamReader(File.OpenRead("edity/layouts/default.html")))
                            {
                                return parsedResponse(source, template, environment);
                            }
                        default:
                            throw new FileNotFoundException();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return this.statusCodeResponse(HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return this.statusCodeResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            try
            {
                MultipartMemoryStreamProvider x = await request.Content.ReadAsMultipartAsync();
                String file = request.RequestUri.AbsolutePath.Remove(0, UploadPath.Length + 1); //Starts with a slash where our defined path does not so + 1
                String directory = Path.GetDirectoryName(file);
                if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
                {
                    await x.Contents[0].CopyToAsync(stream);
                }
                return this.statusCodeResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return this.statusCodeResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage parsedResponse(StreamReader markdown, StreamReader template, TemplateEnvironment environment)
        {
            DocumentRenderer dr = new DocumentRenderer(template.ReadToEnd(), environment);
            return this.htmlResponse(dr.getDocument(markdown.ReadToEnd()));
        }

        public HttpResponseMessage viewMarkdownResponse(StreamReader markdown)
        {
            return this.htmlResponse($@"<html><head><title>{this.currentFile}</title><meta charset=""utf-8"" /></head><body><pre>{markdown.ReadToEnd()}</pre></body></html>");
        }
    }
}