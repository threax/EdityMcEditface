using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        /// <summary>
        /// Try mulitplie times to delete a directory since this fails a lot.
        /// </summary>
        /// <param name="dir"></param>
        public static void MultiTryDirDelete(String dir)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(100); //Small timeout if we got this far
                        Directory.Delete(dir, true); //Last one will throw if needed
                    }
                }
            }
        }

        public void BuildSite()
        {
            //Handle output folder
            MultiTryDirDelete(settings.OutDir);

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
        }
    }
}
