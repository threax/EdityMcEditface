using Newtonsoft.Json;
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
    public class FileFinder : IFileFinder
    {
        private String projectPath;
        private String projectFilePath;
        private Lazy<EdityProject> project;
        private FileFinder next;

        public FileFinder(String projectPath, FileFinder next = null, String projectFilePath = "edity/edity.json")
        {
            project = new Lazy<EdityProject>(loadProject);

            this.projectPath = Path.GetFullPath(projectPath);
            this.projectFilePath = projectFilePath;
            this.next = next;
        }

        /// <summary>
        /// Erase a file in the project. Does not erase files in the backup location.
        /// </summary>
        /// <param name="file"></param>
        public void EraseProjectFile(string file)
        {
            var fullPath = NormalizePath(file);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else if(next != null)
            {
                next.EraseProjectFile(file);
            }
        }

        /// <summary>
        /// Erase a page in the project. Does not erase files in the backup location.
        /// Will erase all linked files, the .html, .json, .css and .js files.
        /// </summary>
        /// <param name="file"></param>
        public void ErasePage(string file)
        {
            var fullPath = NormalizePath(file);
            if (File.Exists(fullPath))
            {
                var jsFile = getPageFile(fullPath, ".js");
                if (jsFile != null)
                {
                    File.Delete(jsFile);
                }
                var cssFile = getPageFile(fullPath, ".css");
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
            else if(next != null)
            {
                next.ErasePage(file);
            }
        }

        /// <summary>
        /// Determine if a layout exists in the layouts folder.
        /// </summary>
        /// <param name="layoutName"></param>
        /// <returns></returns>
        public bool DoesLayoutExist(String layoutName)
        {
            var layoutFileName = getLayoutFile(layoutName);
            var fullPath = NormalizePath(layoutFileName);
            if (File.Exists(fullPath))
            {
                return true;
            }
            else if(next != null)
            {
                return next.DoesLayoutExist(layoutName);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Open a stream to read a file given a path.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <returns>A stream to the requested file.</returns>
        public Stream ReadFile(String file)
        {
            var fullPath = NormalizePath(file);
            if (File.Exists(fullPath))
            {
                return File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else if(next != null)
            {
                return next.ReadFile(file);
            }
            else
            {
                throw new FileNotFoundException($"Cannot find file to read {file}", file);
            }
        }

        /// <summary>
        /// Open a stream to write to a file given a path.
        /// Will create any directories needed to write the file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <returns>A stream to the requested file.</returns>
        public Stream WriteFile(String file)
        {
            var savePath = NormalizePath(file);
            String directory = Path.GetDirectoryName(savePath);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return File.Open(savePath, FileMode.Create, FileAccess.Write);

            //Currently no chain, always write to top level, will add this later
        }

        /// <summary>
        /// Delete a project file.
        /// </summary>
        /// <param name="file">The name of the file to delete.</param>
        public void DeleteFile(String file)
        {
            var fullPath = NormalizePath(file);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else if(next != null)
            {
                next.DeleteFile(file);
            }
        }

        /// <summary>
        /// Delete a folder
        /// </summary>
        /// <param name="folder">The name of the folder to delete.</param>
        public void DeleteFolder(String folder)
        {
            var fullPath = NormalizePath(folder);
            if (Directory.Exists(fullPath))
            {
                //Do a couple trys
                try
                {
                    Directory.Delete(fullPath, true);
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(fullPath, true);
                    }
                    catch (Exception)
                    {
                        //Third time throws if there is a problem.
                        Directory.Delete(fullPath, true);
                    }
                }
            }
            else if(next != null)
            {
                next.DeleteFolder(folder);
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
                var templatePath = NormalizePath("edity/templates");
                if (Directory.Exists(templatePath))
                {
                    query = Directory.EnumerateFiles(templatePath).Select(t => new Template()
                    {
                        Path = Path.ChangeExtension(getUrlFromSystemPath(t), null)
                    });
                }

                if(next != null)
                {
                    var nextQuery = next.Templates;
                    if(nextQuery != null)
                    {
                        if(query == null)
                        {
                            query = nextQuery;
                        }
                        else
                        {
                            query = query.Concat(nextQuery);
                        }
                    }
                }

                return query;
            }
        }

        /// <summary>
        /// Copy the contetnt defined in the project to the outDir
        /// </summary>
        /// <param name="outDir"></param>
        public void CopyProjectContent(String outDir)
        {
            CopyProjectContent(outDir, Project);
        }

        private void CopyProjectContent(String outDir, EdityProject edityProject)
        {
            foreach (var file in edityProject.AdditionalContent.Concat(new string[] { "AutoUploads" }))
            {
                var realPath = NormalizePath(file);
                if (File.Exists(realPath))
                {
                    copyFileIfNotExists(realPath, safePathCombine(outDir, file));
                }
                else if (Directory.Exists(realPath))
                {
                    foreach (var dirFile in Directory.EnumerateFiles(realPath, "*", SearchOption.AllDirectories))
                    {
                        var relativePath = dirFile.Substring(realPath.Length);
                        copyFileIfNotExists(dirFile, safePathCombine(outDir, file, relativePath));
                    }
                }
            }

            if (next != null)
            {
                next.CopyProjectContent(outDir, edityProject);
            }
        }

        /// <summary>
        /// Copy the dependency files for the current page stack.
        /// </summary>
        /// <param name="fileFinder"></param>
        public void CopyDependencyFiles(String baseOutDir, PageStack pageStack)
        {
            CopyDependencyFiles(baseOutDir, pageStack, new HashSet<string>());
        }

        private void CopyDependencyFiles(String baseOutDir, PageStack pageStack, HashSet<String> copiedContentFiles)
        {
            foreach (var page in pageStack.Pages)
            {
                if (!String.IsNullOrEmpty(page.PageCssPath))
                {
                    var cssPath = NormalizePath(page.PageCssPath);
                    if (!copiedContentFiles.Contains(page.PageCssPath) && isValidPhysicalFile(page.PageCssPath) && File.Exists(cssPath))
                    {
                        copyFileIfNotExists(cssPath, safePathCombine(baseOutDir, page.PageCssPath));
                        copiedContentFiles.Add(page.PageCssPath);
                    }
                }
                if (!String.IsNullOrEmpty(page.PageScriptPath))
                {
                    var scriptPath = NormalizePath(page.PageScriptPath);
                    if (!copiedContentFiles.Contains(page.PageScriptPath) && isValidPhysicalFile(page.PageScriptPath) && File.Exists(scriptPath))
                    {
                        copyFileIfNotExists(scriptPath, safePathCombine(baseOutDir, page.PageScriptPath));
                        copiedContentFiles.Add(page.PageScriptPath);
                    }
                }
                foreach (var content in pageStack.LinkedContentFiles)
                {
                    var fullContentPath = NormalizePath(content);
                    if (!copiedContentFiles.Contains(content) && isValidPhysicalFile(content) && File.Exists(fullContentPath))
                    {
                        copyFileIfNotExists(fullContentPath, safePathCombine(baseOutDir, content));
                        copiedContentFiles.Add(content);
                    }
                }
            }

            if(next != null)
            {
                next.CopyDependencyFiles(baseOutDir, pageStack, copiedContentFiles);
            }
        }

        public IEnumerable<String> EnumerateContentFiles(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var fullPath = NormalizePath(path);
            var removeLength = projectPath.Length;
            return Directory.EnumerateFiles(fullPath, searchPattern, searchOption).Select(s => s.Substring(removeLength));
        }

        public IEnumerable<String> EnumerateContentDirectories(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var fullPath = NormalizePath(path);
            var removeLength = projectPath.Length;
            return Directory.EnumerateDirectories(fullPath, searchPattern, searchOption).Select(s => s.Substring(removeLength));
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
        public PageStackItem LoadPageStackLayout(String layout)
        {
            var layoutPath = getLayoutFile(layout);
            var realPath = NormalizePath(layoutPath);
            if (!File.Exists(realPath))
            {
                if(next != null)
                {
                    return next.LoadPageStackLayout(layout);
                }
                else
                {
                    throw new FileNotFoundException($"Cannot find page stack file {layoutPath}", layoutPath);
                }
            }
            layoutPath = layoutPath.EnsureStartingPathSlash();
            return loadPageStackFile(layoutPath, realPath);
        }

        /// <summary>
        /// Load a page stack item as content, this will not attempt to derive a file name
        /// and will use what is passed in. It will also only load content from the main project
        /// folder, not the backup location.
        /// </summary>
        /// <returns></returns>
        public PageStackItem LoadPageStackContent(String path)
        {
            var realPath = NormalizePath(path);
            if (!File.Exists(realPath))
            {
                if(next != null)
                {
                    return next.LoadPageStackContent(path);
                }
                else
                {
                    throw new FileNotFoundException($"Cannot find page stack file {path}", path);
                }
            }
            return loadPageStackFile(path.EnsureStartingPathSlash(), realPath);
        }

        private static string getLayoutFile(String layoutName)
        {
            return $"edity/layouts/{layoutName}";
        }

        private static String getPageDefinitionFile(String file)
        {
            return Path.ChangeExtension(file, "json");
        }

        /// <summary>
        /// Load a page definition. This only looks in the project folder for the definition, not the
        /// backup location.
        /// </summary>
        /// <param name="fileInfo">The file info to use to find the PageDefinition file.</param>
        /// <returns>The PageDefinition for the page. Will be a default instance if the file does not exist.</returns>
        public PageDefinition GetProjectPageDefinition(TargetFileInfo fileInfo)
        {
            return getPageDefinition(fileInfo.HtmlFile);
        }

        private PageDefinition getPageDefinition(String file)
        {
            String settingsPath = getPageDefinitionFile(NormalizePath(file));
            PageDefinition pageSettings;
            if (File.Exists(settingsPath))
            {
                using (var stream = new StreamReader(File.Open(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    pageSettings = JsonConvert.DeserializeObject<PageDefinition>(stream.ReadToEnd());
                }
            }
            else if(next != null)
            {
                return next.getPageDefinition(file);
            }
            else
            {
                return new PageDefinition();
            }

            return pageSettings;
        }

        public void SavePageDefinition(PageDefinition definition, TargetFileInfo fileInfo)
        {
            var outputFile = getPageDefinitionFile(NormalizePath(fileInfo.HtmlFile));
            using (var outStream = new StreamWriter(WriteFile(outputFile)))
            {
                JsonWriter.Serialize(definition, outStream);
            }

            //No Chain, this always writes to this directory.
        }

        /// <summary>
        /// Determine if a path is valid for writing and not protected. The file does not have to exist.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path can be written, false otherwise.</returns>
        public bool IsValidWritablePath(String path)
        {
            try
            {
                NormalizePath(path);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the path passed in relative to the project path. This can be used to get the in project part
        /// of a path from a path relative to a parent directory that is outside the project. If the path
        /// is not in the project path a NotSupportedException is thrown.
        /// </summary>
        /// <param name="path">The path to lookup.</param>
        /// <returns></returns>
        public String GetProjectRelativePath(String path)
        {
            return NormalizePath(path).Substring(projectPath.Length + 1);

            //No chain, only operates on paths
        }

        /// <summary>
        /// Get the page file from the associated link.
        /// </summary>
        /// <param name="file">The link to the page file.</param>
        /// <param name="extension">The extension of the file to look for.</param>
        /// <returns>The matching linkFile name or null if it is not found.</returns>
        private String getPageFile(String file, String extension)
        {
            String realPath = Path.ChangeExtension(NormalizePath(file), extension);
            if (File.Exists(realPath))
            {
                return Path.ChangeExtension(file, extension);
            }
            else if(next != null)
            {
                return next.getPageFile(file, extension);
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
            else if(next != null)
            {
                return next.getUrlFromSystemPath(path);
            }
            return path;
        }

        private static bool isValidPhysicalFile(String file)
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

        private static String safePathCombine(params string[] paths)
        {
            for (int i = 1; i < paths.Length; ++i)
            {
                paths[i] = paths[i].TrimStartingPathChars();
            }
            return Path.Combine(paths);
        }

        private static void copyFileIfNotExists(String source, String dest)
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

        private EdityProject loadProject()
        {
            String projectStr = "";
            String file = NormalizePath(projectFilePath);
            using (var reader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                projectStr = reader.ReadToEnd();
            }

            var project = JsonConvert.DeserializeObject<EdityProject>(projectStr);

            //Merge in all projects
            if(next != null)
            {
                var nextProject = next.Project;
                if (nextProject != null)
                {
                    project.merge(nextProject);
                }
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
                    PageDefinition = getPageDefinition(path),
                    PageScriptPath = getPageFile(path, "js"),
                    PageCssPath = getPageFile(path, "css"),
                };
            }
        }

        /// <summary>
        /// Get the full path of path in the project directory.
        /// The file does not need to exist, and this function does not check for that.
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the path specified is outside of the project path. This can happen if there are ../ items in
        /// the path. By making sure to call NormalizePath first paths can be checked for safety.
        /// </exception>
        /// <returns>The normalized path or null if the path does not exist inside this file finder's folder.</returns>
        private String NormalizePath(String path)
        {
            path = path.TrimStartingPathChars();
            var fullPath = Path.GetFullPath(Path.Combine(projectPath, path));
            //This next line seems weird, but it handles the path having being something like c:\safepath\..\windows\criticalfile.xml
            //When GetFullPath processes that it will be c:\windows\criticalfile.xml, which we then want to deny access to.
            var withinPath = fullPath.StartsWith(projectPath + Path.DirectorySeparatorChar) || fullPath == projectPath;
            if (!withinPath)
            {
                throw new NotSupportedException("Cannot read from directory outside of the defined project paths.");
            }
            return fullPath;
        }
    }
}
