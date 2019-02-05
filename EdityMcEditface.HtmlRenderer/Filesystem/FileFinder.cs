using EdityMcEditface.HtmlRenderer.FileInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Filesystem
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
        private IFileFinderPermissions permissions;
        private IFileStreamManager fileStreamManager;
        private IDraftManager draftManager;

        public FileFinder(String projectPath, IFileFinderPermissions permissions, FileFinder next = null, IFileStreamManager fileStreamManager = null, IDraftManager draftManager = null, String projectFilePath = "edity/edity.json")
        {
            this.fileStreamManager = fileStreamManager;
            if(this.fileStreamManager == null)
            {
                this.fileStreamManager = new FileStreamManager();
            }

            this.draftManager = draftManager;
            if(this.draftManager == null)
            {
                this.draftManager = new NoDrafts();
            }

            project = new Lazy<EdityProject>(loadProject);

            this.projectPath = Path.GetFullPath(projectPath);
            this.projectFilePath = projectFilePath;
            this.next = next;
            this.permissions = permissions;
        }

        /// <summary>
        /// Erase a page in the project. Does not erase files in the backup location.
        /// Will erase all linked files, the .html, .json, .css and .js files.
        /// </summary>
        /// <param name="file">The path to the html file.</param>
        public void ErasePage(string file)
        {
            bool goNext = true;
            var physicalPath = NormalizePath(file);
            if (permissions.AllowWrite(this, file, physicalPath))
            {
                if (File.Exists(physicalPath))
                {
                    var jsFile = getPageFile(file, ".js");
                    if (jsFile != null)
                    {
                        File.Delete(jsFile);
                    }

                    var cssFile = getPageFile(file, ".css");
                    if (cssFile != null)
                    {
                        File.Delete(cssFile);
                    }

                    var settingsFile = NormalizePath(getPageDefinitionFile(file));
                    if (File.Exists(settingsFile))
                    {
                        File.Delete(settingsFile);
                    }

                    draftManager.PageErased(file, physicalPath);
                    File.Delete(physicalPath);

                    goNext = false;
                }
            }

            if (goNext && next != null)
            {
                next.ErasePage(file);
            }
        }

        /// <summary>
        /// Send the specified file to draft.
        /// </summary>
        /// <param name="file"></param>
        public void SendToDraft(String file)
        {
            var normalized = NormalizePath(file);

            var goNext = !draftManager.SendPageToDraft(file, normalized, this);

            if (goNext && next != null)
            {
                next.SendToDraft(file);
            }
        }

        /// <summary>
        /// Get all files that are draftable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetAllDraftables()
        {
            var query = draftManager.GetAllDraftables(this);
            if(next != null)
            {
                query = query.Concat(next.GetAllDraftables());
            }
            return query;
        }

        /// <summary>
        /// Get the draft status of a file. This will tell you if the file's draft matches its latest version
        /// and when the file was last drafted.
        /// </summary>
        /// <param name="file">The file to lookup draft status for.</param>
        /// <returns>The DraftInfo with the file's draft status.</returns>
        public DraftInfo GetDraftStatus(String file)
        {
            var normalized = NormalizePath(file);

            var status = draftManager.GetDraftStatus(file, normalized, this);

            //If the status was null and we have a next in the chain use it.
            if (status == null && next != null)
            {
                status = next.GetDraftStatus(file);
            }

            //Check the next result, if it is still null create a default
            if (status == null)
            {
                status = new DraftInfo(null, DraftStatus.NeverDrafted, file.TrimStartingPathChars());
            }

            return status;
        }

        /// <summary>
        /// Determine if a layout exists in the layouts folder.
        /// </summary>
        /// <param name="layoutName"></param>
        /// <returns></returns>
        public bool DoesLayoutExist(String layoutName)
        {
            var layoutFileName = getLayoutFile(layoutName);
            var physicalPath = NormalizePath(layoutFileName);
            if (permissions.AllowRead(this, layoutFileName, physicalPath))
            {
                if (File.Exists(physicalPath))
                {
                    return true;
                }
            }

            if (next != null)
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
            try
            {
                var physicalPath = NormalizePath(file);
                if (permissions.AllowRead(this, file, physicalPath))
                {
                    if (File.Exists(physicalPath))
                    {
                        return this.fileStreamManager.OpenReadStream(file, physicalPath);
                    }
                }

                if (next != null)
                {
                    return next.ReadFile(file);
                }
                else
                {
                    throw new PageStackFileNotFoundException($"Cannot find file to read {file}", file);
                }
            }
            catch(PageStackFileNotFoundException ex)
            {
                ex.AddSearchLocation(NormalizePath(file));
                throw;
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
            var physicalPath = NormalizePath(file);
            if (permissions.AllowWrite(this, file, physicalPath))
            {
                String directory = Path.GetDirectoryName(physicalPath);
                if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return File.Open(physicalPath, FileMode.Create, FileAccess.Write);
            }

            if (next != null)
            {
                return next.WriteFile(file);
            }
            else
            {
                throw new InvalidOperationException($"Cannot write file {file} no file finder can write to its path.");
            }
        }

        /// <summary>
        /// Delete a project file.
        /// </summary>
        /// <param name="file">The name of the file to delete.</param>
        public void DeleteFile(String file)
        {
            bool goNext = true;
            var physicalPath = NormalizePath(file);
            if (permissions.AllowWrite(this, file, physicalPath))
            {
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    goNext = false;
                }
            }

            if (goNext && next != null)
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
            bool goNext = true;
            var physicalPath = NormalizePath(folder);
            if (permissions.AllowWrite(this, folder, physicalPath))
            {
                if (Directory.Exists(physicalPath))
                {
                    //Do a couple trys
                    try
                    {
                        Directory.Delete(physicalPath, true);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            Directory.Delete(physicalPath, true);
                        }
                        catch (Exception)
                        {
                            //Third time throws if there is a problem.
                            Directory.Delete(physicalPath, true);
                        }
                    }
                }
            }
            if (goNext && next != null)
            {
                next.DeleteFolder(folder);
            }
        }

        const string TemplatePath = "edity/templates";

        /// <summary>
        /// Find the templates in the current project directory.
        /// </summary>
        public IEnumerable<Template> Templates
        {
            get
            {
                IEnumerable<Template> query = null;
                var physicalPath = NormalizePath(TemplatePath);
                if (permissions.AllowRead(this, TemplatePath, physicalPath))
                {
                    if (Directory.Exists(physicalPath))
                    {
                        query = Directory.EnumerateFiles(physicalPath).Select(t => new Template()
                        {
                            Path = Path.ChangeExtension(getUrlFromSystemPath(t), null)
                        });
                    }
                }

                if (next != null)
                {
                    var nextQuery = next.Templates;
                    if (nextQuery != null)
                    {
                        if (query == null)
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
                var physicalPath = NormalizePath(file);
                if (permissions.AllowOutputCopy(this, file, physicalPath))
                {
                    if (File.Exists(physicalPath))
                    {
                        copyFileIfNotExists(file, physicalPath, safePathCombine(outDir, file));
                    }
                    else if (Directory.Exists(physicalPath))
                    {
                        foreach (var dirFile in Directory.EnumerateFiles(physicalPath, "*", SearchOption.AllDirectories))
                        {
                            var relativePath = dirFile.Substring(physicalPath.Length);
                            copyFileIfNotExists(relativePath, dirFile, safePathCombine(outDir, file, relativePath));
                        }
                    }
                }
            }

            if (next != null)
            {
                next.CopyProjectContent(outDir, edityProject);
            }
        }

        /// <summary>
        /// Copy the linked files for the current page stack.
        /// </summary>
        /// <param name="baseOutDir">The output dir.</param>
        /// <param name="pageStack">The page stack to copy files for.</param>
        public void CopyLinkedFiles(String baseOutDir, PageStack pageStack)
        {
            CopyLinkedFiles(baseOutDir, pageStack, new HashSet<string>());
        }

        private void CopyLinkedFiles(String baseOutDir, PageStack pageStack, HashSet<String> copiedFiles)
        {
            foreach (var page in pageStack.Pages)
            {
                if (!String.IsNullOrEmpty(page.PageCssPath))
                {
                    CopyLinkedFile(baseOutDir, copiedFiles, page.PageCssPath);
                }
                if (!String.IsNullOrEmpty(page.PageScriptPath))
                {
                    CopyLinkedFile(baseOutDir, copiedFiles, page.PageScriptPath);
                }
            }

            foreach (var content in pageStack.LinkedContentFiles)
            {
                CopyLinkedFile(baseOutDir, copiedFiles, content);
            }

            if (next != null)
            {
                next.CopyLinkedFiles(baseOutDir, pageStack, copiedFiles);
            }
        }

        private void CopyLinkedFile(string baseOutDir, HashSet<string> copiedFiles, string file)
        {
            var physicalPath = NormalizePath(file);
            if (!copiedFiles.Contains(file) && isValidPhysicalFile(file) && permissions.AllowOutputCopy(this, file, physicalPath))
            {
                if (File.Exists(physicalPath))
                {
                    bool within;
                    var fullDestPath = NormalizePath(file, baseOutDir, out within);
                    if (within)
                    {
                        copyFileIfNotExists(file, physicalPath, fullDestPath);
                        copiedFiles.Add(file);
                    }
                }
            }
        }

        public IEnumerable<String> EnumerateContentFiles(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var seenPaths = new HashSet<String>();
            return EnumerateContentFiles(seenPaths, path, searchPattern, searchOption).ToList(); //Have to use tolist or else this enumerable is not reentrant
        }

        private IEnumerable<String> EnumerateContentFiles(HashSet<String> seenPaths, String path, String searchPattern, SearchOption searchOption)
        {
            var fullPath = NormalizePath(path);
            var removeLength = projectPath.Length;
            IEnumerable<String> query;
            if (Directory.Exists(fullPath))
            {
                query = Directory.EnumerateFiles(fullPath, searchPattern, searchOption).Where(physicalPath =>
                {
                    var relativePath = physicalPath.Substring(removeLength);
                    if (seenPaths.Contains(relativePath) || !permissions.TreatAsContent(this, relativePath, physicalPath))
                    {
                        return false;
                    }
                    seenPaths.Add(relativePath);
                    return true;
                })
                .Select(s => s.Substring(removeLength));
            }
            else
            {
                query = new String[0];
            }

            if (next != null)
            {
                query = query.Concat(next.EnumerateContentFiles(seenPaths, path, searchPattern, searchOption));
            }
            return query;
        }

        public IEnumerable<String> EnumerateContentDirectories(String path, String searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var seenPaths = new HashSet<String>();
            return EnumerateContentDirectories(seenPaths, path, searchPattern, searchOption).ToList(); //Have to use tolist or else this enumerable is not reentrant
        }

        public IEnumerable<String> EnumerateContentDirectories(HashSet<String> seenPaths, String path, String searchPattern, SearchOption searchOption)
        {
            var fullPath = NormalizePath(path);
            var removeLength = projectPath.Length;
            IEnumerable<String> query;
            if (Directory.Exists(fullPath))
            {
                query = Directory.EnumerateDirectories(fullPath, searchPattern, searchOption).Where(physicalPath =>
                {
                    var relativePath = physicalPath.Substring(removeLength);
                    if (seenPaths.Contains(relativePath) || !permissions.TreatAsContent(this, relativePath, physicalPath))
                    {
                        return false;
                    }
                    seenPaths.Add(relativePath);
                    return true;
                })
                .Select(s => s.Substring(removeLength));
            }
            else
            {
                query = new String[0];
            }

            if (next != null)
            {
                query = query.Concat(next.EnumerateContentDirectories(seenPaths, path, searchPattern, searchOption));
            }
            return query;
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
            try
            {
                var layoutPath = getLayoutFile(layout);
                var physicalPath = NormalizePath(layoutPath);
                if (permissions.AllowRead(this, layoutPath, physicalPath))
                {
                    if (File.Exists(physicalPath))
                    {
                        layoutPath = layoutPath.EnsureStartingPathSlash();
                        return loadPageStackFile(layoutPath, physicalPath);
                    }
                }

                if (next != null)
                {
                    return next.LoadPageStackLayout(layout);
                }
                else
                {
                    throw new PageStackFileNotFoundException($"Cannot find page stack file {layoutPath}", layoutPath);
                }
            }
            catch (PageStackFileNotFoundException ex)
            {
                ex.AddSearchLocation(NormalizePath(getLayoutFile(layout)));
                throw;
            }
        }

        /// <summary>
        /// Load a page stack item as content, this will not attempt to derive a file name
        /// and will use what is passed in.
        /// </summary>
        /// <returns></returns>
        public PageStackItem LoadPageStackContent(String path)
        {
            try
            {
                var physicalPath = NormalizePath(path);
                if (permissions.AllowRead(this, path, physicalPath))
                {
                    if (File.Exists(physicalPath))
                    {
                        return loadPageStackFile(path.EnsureStartingPathSlash(), physicalPath);
                    }
                }

                if (next != null)
                {
                    return next.LoadPageStackContent(path);
                }
                else
                {
                    throw new PageStackFileNotFoundException($"Cannot find page stack file {path}", path);
                }
            }
            catch (PageStackFileNotFoundException ex)
            {
                ex.AddSearchLocation(NormalizePath(path));
                throw;
            }
        }

        /// <summary>
        /// Load a page definition.
        /// </summary>
        /// <param name="fileInfo">The file info to use to find the PageDefinition file.</param>
        /// <returns>The PageDefinition for the page. Will be a default instance if the file does not exist.</returns>
        public PageDefinition GetProjectPageDefinition(ITargetFileInfo fileInfo)
        {
            return getProjectPageDefinition(fileInfo.HtmlFile);
        }

        private PageDefinition getProjectPageDefinition(String file)
        {
            var pageDefPath = getPageDefinitionFile(file);
            var physicalPath = NormalizePath(pageDefPath);

            if (permissions.AllowRead(this, pageDefPath, physicalPath))
            {
                if (File.Exists(physicalPath))
                {
                    using (var stream = new StreamReader(this.fileStreamManager.OpenReadStream(pageDefPath, physicalPath)))
                    {
                        return JsonConvert.DeserializeObject<PageDefinition>(stream.ReadToEnd());
                    }
                }
            }

            if (next != null)
            {
                return next.getProjectPageDefinition(file);
            }
            else
            {
                return new PageDefinition();
            }
        }

        public void SavePageDefinition(PageDefinition definition, ITargetFileInfo fileInfo)
        {
            var pageDefFile = getPageDefinitionFile(fileInfo.HtmlFile);
            var physicalPath = NormalizePath(pageDefFile);

            if (permissions.AllowWrite(this, pageDefFile, physicalPath))
            {
                using (var outStream = new StreamWriter(WriteFile(pageDefFile)))
                {
                    JsonWriter.Serialize(definition, outStream);
                }
            }
            else if (next != null)
            {
                next.SavePageDefinition(definition, fileInfo);
            }
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
            catch (NotSupportedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            //No chain, only operates on paths
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
            var changedExtension = Path.ChangeExtension(file, extension);
            String physicalPath = NormalizePath(changedExtension);
            if (permissions.AllowRead(this, changedExtension, physicalPath))
            {
                if (File.Exists(physicalPath))
                {
                    return changedExtension;
                }
            }

            if (next != null)
            {
                return next.getPageFile(file, extension);
            }
            return null;
        }

        private String getUrlFromSystemPath(String path)
        {
            //Does not need permissions, only effects paths by trying to remove the folders for this file finder, if it fails it will chain
            var fullPath = Path.GetFullPath(path);
            var fullProjectPath = Path.GetFullPath(projectPath);
            if (fullPath.StartsWith(fullProjectPath))
            {
                return path.Substring(fullProjectPath.Length);
            }
            else if (next != null)
            {
                return next.getUrlFromSystemPath(path);
            }
            return path;
        }

        private EdityProject loadProject()
        {
            EdityProject project = null;
            String physicalFilePath = NormalizePath(projectFilePath);
            if (permissions.AllowRead(this, projectFilePath, physicalFilePath))
            {
                if (File.Exists(physicalFilePath))
                {
                    String projectStr = "";
                    using (var reader = new StreamReader(File.Open(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        projectStr = reader.ReadToEnd();
                    }

                    project = JsonConvert.DeserializeObject<EdityProject>(projectStr);
                }
            }

            //Merge in all projects
            if (next != null)
            {
                var nextProject = next.Project;
                if (nextProject != null)
                {
                    if (project == null)
                    {
                        project = nextProject;
                    }
                    else
                    {
                        project.merge(nextProject);
                    }
                }
            }

            return project;
        }

        private PageStackItem loadPageStackFile(string path, string realPath)
        {
            
                return new PageStackItem(() =>
                {
                    //Defer loading until later, this saves a lot of file reads
                    using (var stream = new StreamReader(this.fileStreamManager.OpenReadStream(path, realPath)))
                    {
                        return stream.ReadToEnd();
                    }
                })
                {
                    PageDefinition = getProjectPageDefinition(path),
                    PageScriptPath = getPageFile(path, "js"),
                    PageCssPath = getPageFile(path, "css"),
                };
        }

        protected String NormalizePath(String path)
        {
            bool withinPath;
            var normalized = NormalizePath(path, projectPath, out withinPath);
            if (!withinPath)
            {
                throw new NotSupportedException("Cannot read from directory outside of the defined project paths.");
            }
            return normalized;
        }

        /// <summary>
        /// Get the full path of path in the project directory.
        /// The file does not need to exist, and this function does not check for that.
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <param name="rootPath">The path to make sure we are under.</param>
        /// <param name="withinPath">Will be set to true if the path is within rootPath.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the path specified is outside of the project path. This can happen if there are ../ items in
        /// the path. By making sure to call NormalizePath first paths can be checked for safety.
        /// </exception>
        /// <returns>The normalized path or null if the path does not exist inside this file finder's folder.</returns>
        private static String NormalizePath(String path, String rootPath, out bool withinPath)
        {
            //Remove path roots ~/ or ~\
            if(path.Length > 1 && path[0] == '~' && (path[1] == '\\' || path[1] == '/'))
            {
                path = path.Substring(2);
            }
            path = path.TrimStartingPathChars();
            var fullRootPath = Path.GetFullPath(rootPath);
            var fullPath = Path.GetFullPath(Path.Combine(fullRootPath, path));
            //This next line seems weird, but it handles the path having being something like c:\safepath\..\windows\criticalfile.xml
            //When GetFullPath processes that it will be c:\windows\criticalfile.xml, which we then want to deny access to.
            withinPath = fullPath.StartsWith(fullRootPath + Path.DirectorySeparatorChar) || fullPath == fullRootPath;
            return fullPath;
        }

        private static string getLayoutFile(String layoutName)
        {
            return $"edity/layouts/{layoutName}";
        }

        private static String getPageDefinitionFile(String file)
        {
            return Path.ChangeExtension(file, "json");
        }

        private static String safePathCombine(params string[] paths)
        {
            for (int i = 1; i < paths.Length; ++i)
            {
                paths[i] = paths[i].TrimStartingPathChars();
            }
            return Path.Combine(paths);
        }

        private void copyFileIfNotExists(String source, String physicalSource, String physicalDest)
        {
            if (!File.Exists(physicalDest))
            {
                var destDir = Path.GetDirectoryName(physicalDest);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                this.fileStreamManager.CopyFile(source, physicalSource, physicalDest);
            }
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

        public string LoadSection(string path)
        {
            path = Path.Combine("edity/layouts/sections", path.TrimStartingPathChars());
            using(var reader = new StreamReader(this.ReadFile(path)))
            {
                return reader.ReadToEnd();
            }
        }

        public IEnumerable<string> GetPageContentFiles(string file)
        {
            var linked = getPageFile(file, "js");
            if(linked != null)
            {
                yield return linked;
            }
            linked = getPageFile(file, "css");
            if (linked != null)
            {
                yield return linked;
            }
            linked = getPageFile(file, "json");
            if (linked != null)
            {
                yield return linked;
            }
        }
    }
}
