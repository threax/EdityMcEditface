using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.Models.Compiler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]/[action]")]
    public class CompileController : Controller
    {
        private CompilerSettings settings;

        public CompileController(CompilerSettings settings)
        {
            this.settings = settings;
        }

        [HttpPost]
        public void Post()
        {
            //Handle output folder
            if (Directory.Exists(settings.OutDir))
            {
                Directory.Delete(settings.OutDir, true);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Directory.CreateDirectory(settings.OutDir);

            var fileFinder = new FileFinder(settings.InDir, settings.BackupPath);

            var compilers = ContentCompilerFactory.CreateCompilers(settings.InDir, settings.OutDir, settings.BackupPath, fileFinder.Project.Compilers);

            foreach (var file in Directory.EnumerateFiles(settings.InDir, "*.html", SearchOption.AllDirectories))
            {
                var relativeFile = FileFinder.TrimStartingPathChars(file.Substring(settings.InDir.Length));
                if (!relativeFile.StartsWith(settings.EdityDir, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var compiler in compilers)
                    {
                        compiler.buildPage(relativeFile);
                    }
                }
            }

            foreach (var compiler in compilers)
            {
                compiler.copyProjectContent();
            }

            sw.Stop();
        }
    }
}
