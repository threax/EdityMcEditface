using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class DirectOutputSiteBuilder : ISiteBuilder
    {
        private SiteBuilderSettings settings;
        private List<IBuildTask> preBuildTasks = new List<IBuildTask>();
        private List<IBuildTask> postBuildTasks = new List<IBuildTask>();
        private List<IPublishTask> publishTasks = new List<IPublishTask>();
        private IContentCompilerFactory contentCompilerFactory;
        private IFileFinder fileFinder;

        private int currentFile;
        private int totalFiles;

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

        public async Task BuildSite()
        {
            //Pre build tasks
            foreach(var task in preBuildTasks)
            {
                await task.Execute();
            }

            //Handle output folder
            MultiTryDirDelete(settings.OutDir);

            Directory.CreateDirectory(settings.OutDir);

            var compilers = contentCompilerFactory.CreateCompilers(fileFinder, settings.OutDir, fileFinder.Project.Compilers);
            var query = fileFinder.EnumerateContentFiles("/", "*.html", SearchOption.AllDirectories);

            totalFiles = query.Count();

            foreach (var file in query)
            {
                ++currentFile;
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
                await task.Execute();
            }

            //Publish tasks
            foreach (var task in publishTasks)
            {
                await task.Execute();
            }
        }

        public void AddPreBuildTask(IBuildTask task)
        {
            preBuildTasks.Add(task);
        }

        public void AddPostBuildTask(IBuildTask task)
        {
            postBuildTasks.Add(task);
        }

        public void AddPublishTask(IPublishTask task)
        {
            publishTasks.Add(task);
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

        public bool DoesOutputFileExist(String file)
        {
            var fullPath = Path.Combine(settings.OutDir, StringPathExtensions.TrimStartingPathChars(file));
            return File.Exists(fullPath);
        }

        public BuildProgress GetCurrentProgress()
        {
            return new BuildProgress()
            {
                CurrentFile = this.currentFile,
                TotalFiles = this.totalFiles
            };
        }
    }
}
