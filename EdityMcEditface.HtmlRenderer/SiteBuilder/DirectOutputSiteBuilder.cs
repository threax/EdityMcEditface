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
        private List<BuildTask> preBuildTasks = new List<BuildTask>();
        private List<BuildTask> postBuildTasks = new List<BuildTask>();
        private IContentCompilerFactory contentCompilerFactory;
        private IFileFinder fileFinder;

        public DirectOutputSiteBuilder(SiteBuilderSettings settings, IContentCompilerFactory contentCompilerFactory, IFileFinder fileFinder)
        {
            this.contentCompilerFactory = contentCompilerFactory;
            this.settings = settings;
            this.fileFinder = fileFinder;
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
            //Pre build tasks
            foreach(var task in preBuildTasks)
            {
                task.execute();
            }

            //Handle output folder
            MultiTryDirDelete(settings.OutDir);

            Directory.CreateDirectory(settings.OutDir);

            var compilers = contentCompilerFactory.CreateCompilers(fileFinder, settings.OutDir, fileFinder.Project.Compilers);

            foreach (var file in fileFinder.EnumerateContentFiles("/", "*.html", SearchOption.AllDirectories))
            {
                foreach (var compiler in compilers)
                {
                    compiler.buildPage(file);
                }
            }

            foreach (var compiler in compilers)
            {
                compiler.copyProjectContent();
            }

            //Post build tasks
            foreach (var task in postBuildTasks)
            {
                task.execute();
            }
        }

        public void addPreBuildTask(BuildTask task)
        {
            preBuildTasks.Add(task);
        }

        public void addPostBuildTask(BuildTask task)
        {
            postBuildTasks.Add(task);
        }

        public Stream OpenOutputWriteStream(string file)
        {
            var fullPath = Path.Combine(settings.OutDir, StringPathExtensions.TrimStartingPathChars(file));
            var fullDir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
            }
            return File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        }
    }
}
