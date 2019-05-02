﻿using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class SiteBuilder : ISiteBuilder
    {
        private SiteBuilderSettings settings;
        private List<IBuildTask> preBuildTasks = new List<IBuildTask>();
        private List<IBuildTask> postBuildTasks = new List<IBuildTask>();
        private List<IPublishTask> publishTasks = new List<IPublishTask>();
        private List<IPublishTask> postPublishTasks = new List<IPublishTask>();
        private IContentCompilerFactory contentCompilerFactory;
        private IFileFinder fileFinder;
        private String deploymentFolder;

        private int currentFile;
        private int totalFiles;
        private IBuildStatusTracker buildStatusTracker = new BuildStatusTracker();
        private BuildEventArgs args;

        public SiteBuilder(SiteBuilderSettings settings, IContentCompilerFactory contentCompilerFactory, IFileFinder fileFinder, String deploymentFolder)
        {
            this.deploymentFolder = deploymentFolder;
            this.args = new BuildEventArgs(buildStatusTracker, this);
            this.contentCompilerFactory = contentCompilerFactory;
            this.settings = settings;
            this.fileFinder = fileFinder;
        }

        public async Task BuildSite()
        {
            //Pre build tasks
            foreach(var task in preBuildTasks)
            {
                await task.Execute(args);
            }

            args.Tracker.AddMessage("Building website.");
            //Look for .git folder, if one exists move it temporarily
            var gitPath = Path.GetFullPath(Path.Combine(settings.OutDir, ".git"));
            String tempGitPath = null;
            if (Directory.Exists(gitPath))
            {
                //Move git folder out to a temp location
                tempGitPath = Path.GetFullPath(settings.OutDir + "-Git-" + Guid.NewGuid().ToString());
                Directory.Move(gitPath, tempGitPath);
            }

            //Erase output folder
            IOExtensions.MultiTryDirDelete(settings.OutDir);

            //Create output folder
            Directory.CreateDirectory(settings.OutDir);

            //Move git folder back if we moved it from the dest directory
            if (tempGitPath != null)
            {
                Directory.Move(tempGitPath, gitPath);
            }

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
                await task.Execute(args);
            }

            //Publish tasks
            foreach (var task in publishTasks)
            {
                await task.Execute(args);
            }

            //PostPublish tasks
            foreach (var task in postPublishTasks)
            {
                await task.Execute(args);
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

        public void AddPostPublishTask(IPublishTask task)
        {
            postPublishTasks.Add(task);
        }

        public Stream OpenInputReadStream(string file)
        {
            return fileFinder.ReadFile(file);
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

        public Stream OpenOutputParentWriteStream(string file)
        {
            var outDir = settings.OutDir;
            if(deploymentFolder != null)
            {
                //If using a deployment folder, get the parent folder.
                outDir = Path.GetDirectoryName(outDir);
            }
            var fullPath = Path.Combine(outDir, StringPathExtensions.TrimStartingPathChars(file));
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
                TotalFiles = this.totalFiles,
                Messages = buildStatusTracker.GetMessages()
            };
        }

        public String DeploymentSubFolder
        {
            get
            {
                return deploymentFolder;
            }
        }

        public EdityProject Project
        {
            get
            {
                return fileFinder.Project;
            }
        }
    }
}
