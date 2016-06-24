﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    /// <summary>
    /// Loads and processes various files in the project. Used to build
    /// documents from the file system.
    /// </summary>
    public class FileFinder
    {
        private static char[] PathTrimChars = { '\\', '/' };
        /// <summary>
        /// Trim leading / and \ from a string.
        /// </summary>
        /// <param name="input">The path to trim.</param>
        /// <returns>The path without any leading / or \.</returns>
        public static String TrimStartingPathChars(String input)
        {
            return input.TrimStart(PathTrimChars);
        }

        /// <summary>
        /// This is the location of an additional directory to try to serve files from,
        /// best used to serve the default files this app needs to run.
        /// </summary>
        private String backupPath = null;
        private String projectPath;
        private String projectFilePath;
        private Lazy<EdityProject> project;

        public FileFinder(String projectPath, String backupPath, String projectFilePath = "edity/edity.json")
        {
            project = new Lazy<EdityProject>(loadProject);

            this.projectPath = projectPath;
            this.backupPath = backupPath;
            this.projectFilePath = projectFilePath;
        }

        /// <summary>
        /// Determine if a layout exists in the layouts folder.
        /// </summary>
        /// <param name="layoutName"></param>
        /// <returns></returns>
        public bool doesLayoutExist(String layoutName)
        {
            var layoutPath = findRealFile(getLayoutFile(layoutName));
            return layoutPath != null;
        }

        /// <summary>
        /// Open a stream to read a file given a path.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <returns>A stream to the requested file.</returns>
        public Stream readFile(String file)
        {
            var realFile = findRealFile(file);
            if (realFile != null)
            {
                return File.Open(realFile, FileMode.Open, FileAccess.Read);
            }
            throw new FileNotFoundException($"Cannot find file to read {file}", file);
        }

        /// <summary>
        /// Open a stream to write to a file given a path.
        /// Will create any directories needed to write the file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <returns>A stream to the requested file.</returns>
        public Stream writeFile(String file)
        {
            var savePath = getFullProjectPath(file);
            String directory = Path.GetDirectoryName(savePath);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return File.Open(savePath, FileMode.Create, FileAccess.Write);
        }

        /// <summary>
        /// Find the templates in the current project directory.
        /// </summary>
        public IEnumerable<Template> Templates
        {
            get
            {
                return Directory.EnumerateFiles(getFullProjectPath("edity/templates")).Select(t => new Template()
                {
                    Path = Path.ChangeExtension(getUrlFromSystemPath(t), null)
                });
            }
        }

        /// <summary>
        /// Copy the contetnt defined in the project to the outDir
        /// </summary>
        /// <param name="outDir"></param>
        public void copyProjectContent(String outDir)
        {
            foreach (var file in Project.AdditionalContent)
            {
                var realFile = findRealFile(file);
                if (File.Exists(realFile))
                {
                    copyFileIfNotExists(realFile, safePathCombine(outDir, file));
                }
                else
                {
                    //Copy all files from normal and backup location
                    copyFolderContents(getFullProjectPath(file), outDir, file);
                    copyFolderContents(getBackupPath(file), outDir, file);
                }
            }
        }

        /// <summary>
        /// Copy the dependency files for the current page stack.
        /// </summary>
        /// <param name="fileFinder"></param>
        public void copyDependencyFiles(String outDir, PageStack pageStack)
        {
            HashSet<String> copiedContentFiles = new HashSet<string>();

            foreach (var page in pageStack.Pages)
            {
                if (!String.IsNullOrEmpty(page.PageCssPath))
                {
                    copyFileIfNotExists(getFullProjectPath(page.PageCssPath), safePathCombine(outDir, page.PageCssPath));
                }
                if (!String.IsNullOrEmpty(page.PageScriptPath))
                {
                    copyFileIfNotExists(getFullProjectPath(page.PageScriptPath), safePathCombine(outDir, page.PageScriptPath));
                }
                foreach (var content in pageStack.LinkedContentFiles)
                {
                    if (!copiedContentFiles.Contains(content) && isValidPhysicalFile(content))
                    {
                        copyFileIfNotExists(findRealFile(content), safePathCombine(outDir, content));
                    }
                    copiedContentFiles.Add(content);
                }
            }
        }

        public EdityProject Project
        {
            get
            {
                return project.Value;
            }
        }

        /// <summary>
        /// Load the page stack. The pages will be loaded and returned from innermost
        /// to outermost.
        /// </summary>
        /// <returns></returns>
        public PageStackItem loadPageStackItem(String path, bool isLayout)
        {
            if (isLayout)
            {
                path = getLayoutFile(path);
            }
            var realPath = findRealFile(path);
            if(realPath == null)
            {
                throw new FileNotFoundException($"Cannot find page stack file {path}", path);
            }
            using (var layout = new StreamReader(File.OpenRead(realPath)))
            {
                return new PageStackItem()
                {
                    Content = layout.ReadToEnd(),
                    PageDefinition = getPageDefinition(realPath),
                    PageScriptPath = getPageFile(realPath, path, "js"),
                    PageCssPath = getPageFile(realPath, path, "css"),
                };
            }
        }

        /// <summary>
        /// Find the full real file path if the file exists or null if it does not.
        /// </summary>
        /// <param name="file">The file to look for.</param>
        /// <param name="usedBackup">Will be set to true if the backup file was used.</param>
        /// <returns>The real file path or null if the file does not exist in any search folders.</returns>
        private String findRealFile(String file, out bool usedBackup)
        {
            usedBackup = false;

            var realFile = getFullProjectPath(file);
            if (File.Exists(realFile))
            {
                return realFile;
            }

            string backupFileLoc = getBackupPath(file);
            if (File.Exists(backupFileLoc))
            {
                usedBackup = true;
                return Path.GetFullPath(backupFileLoc);
            }

            //Not found, return null
            return null;
        }

        private string getBackupPath(string file)
        {
            file = TrimStartingPathChars(file);
            return Path.Combine(backupPath, file);
        }

        private string getLayoutFile(String layoutName)
        {
            String file = $"edity/layouts/{layoutName}";
            return file;
        }

        private PageDefinition getPageDefinition(String file)
        {
            String settingsPath = Path.ChangeExtension(file, "json");
            PageDefinition pageSettings;
            if (File.Exists(settingsPath))
            {
                using (var stream = new StreamReader(File.Open(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
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
            if (File.Exists(realPath))
            {
                return Path.ChangeExtension(linkFile, extension);
            }
            return null;
        }

        private String getUrlFromSystemPath(String path)
        {
            var fullPath = Path.GetFullPath(path);
            var fullProjectPath = Path.GetFullPath(projectPath);
            if (fullPath.StartsWith(fullProjectPath))
            {
                return path.Substring(fullProjectPath.Length);
            }
            if (backupPath != null)
            {
                fullProjectPath = Path.GetFullPath(backupPath);
                if (fullPath.StartsWith(fullProjectPath))
                {
                    return path.Substring(fullPath.Length - 1);
                }
            }
            return path;
        }

        private bool isValidPhysicalFile(String file)
        {
            try
            {
                Path.GetFullPath(file);
            }
            catch (NotSupportedException)
            {
                return false;
            }
            return true;
        }

        private String safePathCombine(params string[] paths)
        {
            for (int i = 1; i < paths.Length; ++i)
            {
                paths[i] = FileFinder.TrimStartingPathChars(paths[i]);
            }
            return Path.Combine(paths);
        }

        private void copyFileIfNotExists(String source, String dest)
        {
            if (!File.Exists(dest))
            {
                var destDir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                File.Copy(source, dest);
            }
        }

        private void copyFolderContents(string sourceDir, String outDir, String additionalPath)
        {
            if (Directory.Exists(sourceDir))
            {
                foreach (var dirFile in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    var relativePath = dirFile.Substring(sourceDir.Length);
                    copyFileIfNotExists(dirFile, safePathCombine(outDir, additionalPath, relativePath));
                }
            }
        }

        /// <summary>
        /// Get the full path of path in the project directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private String getFullProjectPath(String path)
        {
            path = TrimStartingPathChars(path);
            return Path.GetFullPath(Path.Combine(projectPath, path));
        }

        /// <summary>
        /// Find the full real file path if the file exists or null if it does not.
        /// file name.
        /// </summary>
        /// <param name="file">The file to look for.</param>
        /// <returns>The real file path or null if the file does not exist in any search folders.</returns>
        private String findRealFile(String file)
        {
            bool usedBackup;
            return findRealFile(file, out usedBackup);
        }

        private EdityProject loadProject()
        {
            String projectStr = "";
            bool usedBackup = false;
            String file = findRealFile(projectFilePath, out usedBackup);
            using (var reader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                projectStr = reader.ReadToEnd();
            }

            var project = JsonConvert.DeserializeObject<EdityProject>(projectStr);

            if (!usedBackup)
            {
                //Also load the backup file and merge it in
                //This does load twice if the backup loc is the project loc, but that won't be common
                //and if so can check for it here.
                file = getBackupPath(projectFilePath);
                using (var reader = new StreamReader(System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    projectStr = reader.ReadToEnd();
                }

                var backupProject = JsonConvert.DeserializeObject<EdityProject>(projectStr);

                project.merge(backupProject);
            }

            return project;
        }
    }
}
