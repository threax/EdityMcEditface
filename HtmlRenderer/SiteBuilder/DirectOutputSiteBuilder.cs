using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class DirectOutputSiteBuilder : SiteBuilder
    {
        private SiteBuilderSettings settings;

        public DirectOutputSiteBuilder(SiteBuilderSettings settings)
        {
            this.settings = settings;
        }

        public async Task BuildSite()
        {
            await Task.Run(() =>
            {
                //Handle output folder
                if (Directory.Exists(settings.OutDir))
                {
                    Directory.Delete(settings.OutDir, true);
                }

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
            });
        }
    }
}
