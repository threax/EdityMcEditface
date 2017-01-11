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

            this.projectPath = Path.GetFullPath(projectPath);
            this.backupPath = Path.GetFullPath(backupPath);
            this.projectFilePath = projectFilePath;
        }

        /// <summary>
        /// Erase a file in the project. Does not erase files in the backup location.
        /// </summary>
        /// <param name="file"></param>
        public void eraseProjectFile(string file)
        {
            var fullPath = normalizeProjectPath(file);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <summary>
        /// Erase a page in the project. Does not erase files in the backup location.
        /// Will erase all linked files, the .html, .json, .css and .js files.
        /// </summary>
        /// <param name="file"></param>
        public void erasePage(string file)
        {
            var fullPath = normalizeProjectPath(file);
            if (File.Exists(fullPath))
            {
                var jsFile = getPageFile(fullPath, fullPath, ".js");
                if(jsFile != null)
                {
                    File.Delete(jsFile);
                }
                var cssFile = getPageFile(fullPath, fullPath, ".css");
                if (cssFile != null)
                {
                    File.Delete(cssFile);
                }
                var settingsFile = getPageDefinitionFile(fullPath);
                if (File.Exists(settingsFile))
                {
                    File.Delete(settingsFile);
                }
                File.Delete(fullPath);
            }
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
                return File.Open(realFile, FileMode.Open, FileAccess.Read, FileShare.Read);
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
            var savePath = normalizeProjectPath(file);
            String directory = Path.GetDirectoryName(savePath);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return File.Open(savePath, FileMode.Create, FileAccess.Write);
        }

        /// <summary>
        /// Delete a project file.
        /// </summary>
        /// <param name="file">The name of the file to delete.</param>
        public void deleteFile(String file)
        {
            file = normalizeProjectPath(file);
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Delete a folder
        /// </summary>
        /// <param name="folder">The name of the folder to delete.</param>
        public void deleteFolder(String folder)
        {
            folder = normalizeProjectPath(folder);
            if (Directory.Exists(folder))
            {
                //Do a couple trys
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (Exception)
                    {
                        //Third time throws if there is a problem.
                        Directory.Delete(folder, true);
                    }
                }
            }
        }

        /// <summary>
        /// Find the templates in the current project directory.
        /// </summary>
        public IEnumerable<Template> Templates
        {
            get
            {
                IEnumerable<Template> query = null;
                var templatePath = normalizeProjectPath("edity/templates");
                if (Directory.Exists(templatePath))
                {
                    query = Directory.EnumerateFiles(templatePath).Select(t => new Template()
                    {
                        Path = Path.ChangeExtension(getUrlFromSystemPath(t), null)
                    });
                }
                else
                {
                    templatePath = normalizeBackupPath("edity/templates");
                    if (Directory.Exists(templatePath))
                    {
                        query = Directory.EnumerateFiles(templatePath).Select(t => new Template()
                        {
                            Path = Path.ChangeExtension(getUrlFromSystemPath(t), null)
                        });
                    }
                }
                return query;
            }
        }

        /// <summary>
        /// Copy the contetnt defined in the project to the outDir
        /// </summary>
        /// <param name="outDir"></param>
        public void copyProjectContent(String outDir)
        {
            foreach (var file in Project.AdditionalContent.Concat(new string[]{ "AutoUploads" }))
            {
                var realFile = findRealFile(file);
                if (File.Exists(realFile))
                {
                    copyFileIfNotExists(realFile, safePathCombine(outDir, file));
                }
                else
                {
                    //Copy all files from normal and backup location
                    copyFolderContents(normalizeProjectPath(file), outDir, file);
                    copyFolderContents(normalizeBackupPath(file), outDir, file);
                }
            }
        }

        /// <summary>
        /// Copy the dependency files for the current page stack.
        /// </summary>
        /// <param name="fileFinder"></param>
        public void copyDependencyFiles(String baseOutDir, PageStack pageStack)
        {
            HashSet<String> copiedContentFiles = new HashSet<string>();

            foreach (var page in pageStack.Pages)
            {
                if (!String.IsNullOrEmpty(page.PageCssPath))
                {
                    copyFileIfNotExists(normalizeProjectPath(page.PageCssPath), safePathCombine(baseOutDir, page.PageCssPath));
                }
                if (!String.IsNullOrEmpty(page.PageScriptPath))
                {
                    copyFileIfNotExists(normalizeProjectPath(page.PageScriptPath), safePathCombine(baseOutDir, page.PageScriptPath));
                }
                foreach (var content in pageStack.LinkedContentFiles)
                {
                    if (!copiedContentFiles.Contains(content) && isValidPhysicalFile(content))
                    {
                        copyFileIfNotExists(findRealFile(content), safePathCombine(baseOutDir, content));
                    }
                    copiedContentFiles.Add(content);
                }
            }
        }

        public IEnumerable<String> enumerateFiles(String path)
        {
            var fullPath = normalizeProjectPath(path);
            var removeLength = projectPath.Length;
            return Directory.EnumerateFiles(fullPath).Select(s => s.Substring(removeLength));
        }

        public IEnumerable<String> enumerateDirectories(String path)
        {
            var fullPath = normalizeProjectPath(path);
            var removeLength = projectPath.Length;
            return Directory.EnumerateDirectories(fullPath).Select(s => s.Substring(removeLength));
        }

        public EdityProject Project
        {
            get
            {
                return project.Value;
            }
        }

        /// <summary>
        /// Load a page stack item as a layout, will attempt to locate the 
        /// passed layout path automatically.
        /// </summary>
        /// <returns></returns>
        public PageStackItem loadPageStackLayout(String path)
        {
            path = getLayoutFile(path);
            var realPath = findRealFile(path);
            if (realPath == null)
            {
                throw new FileNotFoundException($"Cannot find page stack file {path}", path);
            }
            if (path[0] != '\\' && path[0] != '/')
            {
                path = '/' + path;
            }
            return loadPageStackFile(path, realPath);
        }

        /// <summary>
        /// Load a page stack item as content, this will not attempt to derive a file name
        /// and will use what is passed in. It will also only load content from the main project
        /// folder, not the backup location.
        /// </summary>
        /// <returns></returns>
        public PageStackItem loadPageStackContent(String path)
        {
            var realPath = normalizeProjectPath(path);
            if (realPath == null)
            {
                throw new FileNotFoundException($"Cannot find page stack file {path}", path);
            }
            if (path[0] != '\\' && path[0] != '/')
            {
                path = '/' + path;
            }
            return loadPageStackFile(path, realPath);
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

            var realFile = normalizeProjectPath(file);
            if (File.Exists(realFile))
            {
                return realFile;
            }

            string backupFileLoc = normalizeBackupPath(file);
            if (File.Exists(backupFileLoc))
            {
                usedBackup = true;
                return backupFileLoc;
            }

            //Not found, return null
            return null;
        }

        private string getLayoutFile(String layoutName)
        {
            String file = $"edity/layouts/{layoutName}";
            return file;
        }

        private String getPageDefinitionFile(String file)
        {
            return Path.ChangeExtension(file, "json");
        }

        /// <summary>
        /// Load a page definition. This only looks in the project folder for the definition, not the
        /// backup location.
        /// </summary>
        /// <param name="fileInfo">The file info to use to find the PageDefinition file.</param>
        /// <returns>The PageDefinition for the page. Will be a default instance if the file does not exist.</returns>
        public PageDefinition getProjectPageDefinition(TargetFileInfo fileInfo)
        {
            return getPageDefinition(normalizeProjectPath(fileInfo.HtmlFile));
        }

        private PageDefinition getPageDefinition(String file)
        { 
            String settingsPath = getPageDefinitionFile(file);
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

        public void savePageDefinition(PageDefinition definition, TargetFileInfo fileInfo)
        {
            var outputFile = getPageDefinitionFile(fileInfo.HtmlFile);
            using (var outStream = new StreamWriter(writeFile(outputFile)))
            {
                JsonWriter.Serialize(definition, outStream);
            }
        }

        /// <summary>
        /// Determine if a path is valid for writing and not protected. The file does not have to exist.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path can be written, false otherwise.</returns>
        public bool isValidWritablePath(String path)
        {
            bool inside;
            normalizeProjectPath(path, out inside);
            return inside;
        }

        /// <summary>
        /// Get the path passed in relative to the project path. This can be used to get the in project part
        /// of a path from a path relative to a parent directory that is outside the project. If the path
        /// is not in the project path a NotSupportedException is thrown.
        /// </summary>
        /// <param name="path">The path to lookup.</param>
        /// <returns></returns>
        public String getProjectRelativePath(String path)
        {
            return normalizeProjectPath(path).Substring(projectPath.Length + 1);
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
                    return path.Substring(fullProjectPath.Length);
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
                file = normalizeBackupPath(projectFilePath);
                using (var reader = new StreamReader(System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    projectStr = reader.ReadToEnd();
                }

                var backupProject = JsonConvert.DeserializeObject<EdityProject>(projectStr);

                project.merge(backupProject);
            }

            return project;
        }

        private PageStackItem loadPageStackFile(string path, string realPath)
        {
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
        /// Get the full path of path in the project directory. Will throw a NotSupportedException if the path is not in the project folder.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private String normalizeProjectPath(String path)
        {
            bool inside;
            var fullPath = normalizeProjectPath(path, out inside);
            if (!inside)
            {
                throw new NotSupportedException($"Cannot load file from directory outside of {projectPath}");
            }
            return fullPath;
        }

        /// <summary>
        /// Normalize a project path, this will fill out withinPath to be true if the path is inside the project path and
        /// false if not.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <param name="withinPath">True if path is inside backupPath.</param>
        /// <returns>The fully expanded path string.</returns>
        private String normalizeProjectPath(String path, out bool withinPath)
        {
            path = TrimStartingPathChars(path);
            var fullPath = Path.GetFullPath(Path.Combine(projectPath, path));
            withinPath = fullPath.StartsWith(projectPath + Path.DirectorySeparatorChar) || fullPath == projectPath;
            return fullPath;
        }

        /// <summary>
        /// Get the backup file path. Will throw a NotSupportedException if the path is not in the backup folder.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private String normalizeBackupPath(String path)
        {
            bool inside;
            var fullPath = normalizeBackupPath(path, out inside);
            if (!inside)
            {
                throw new NotSupportedException($"Cannot load file from directory outside of {backupPath}");
            }
            return fullPath;
        }

        /// <summary>
        /// Normalize a backup path, this will fill out withinPath to be true if the path is inside the backup path and
        /// false if not.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <param name="withinPath">True if path is inside backupPath.</param>
        /// <returns>The fully expanded path string.</returns>
        private String normalizeBackupPath(String path, out bool withinPath)
        {
            path = TrimStartingPathChars(path);
            var fullPath = Path.GetFullPath(Path.Combine(backupPath, path));
            withinPath = fullPath.StartsWith(backupPath + Path.DirectorySeparatorChar) || fullPath == backupPath;
            return fullPath;
        }
    }
}