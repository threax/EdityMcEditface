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
        /// <summary>
        /// This is the location of an additional directory to try to serve files from,
        /// best used to serve the default files this app needs to run.
        /// </summary>
        private String backupPath = null;
        private String projectPath;
        private String projectFilePath;
        private String fileNoExtension;
        private String htmlFile;
        private String directory;
        private String extension;
        private bool isDirectory = false;
        private TemplateEnvironment environment;
        private List<String> templates = new List<string>();

        public FileFinder(String projectPath, String backupPath, String projectFilePath = "edity/edity.json")
        {
            this.projectPath = projectPath;
            this.backupPath = backupPath;
            this.projectFilePath = projectFilePath;
        }

        /// <summary>
        /// Use the specified file as the actual page. This must be called before
        /// most other functions will do anything useful.
        /// </summary>
        /// <param name="file"></param>
        public void useFile(String file)
        {
            file = detectIndexFile(file);

            extension = Path.GetExtension(file).ToLowerInvariant();

            //Fix file name
            fileNoExtension = file;
            if (extension.Length != 0 && fileNoExtension.Length > extension.Length)
            {
                fileNoExtension = fileNoExtension.Remove(fileNoExtension.Length - extension.Length);
            }

            directory = fileNoExtension;
            htmlFile = fileNoExtension + ".html";

            if (string.IsNullOrEmpty(extension))
            {
                if (String.IsNullOrEmpty(directory))
                {
                    directory = ".";
                }

                isDirectory = Directory.Exists(directory) && !File.Exists(htmlFile);
            }

            EdityProject project = loadProject();
            environment = new TemplateEnvironment("/" + fileNoExtension, project);
        }

        public void pushTemplate(String template)
        {
            this.templates.Add(template);
        }

        public void clearTemplates()
        {
            this.templates.Clear();
        }

        public String getFullRealPath(String path)
        {
            return Path.GetFullPath(Path.Combine(projectPath, path));
        }

        public string Extension
        {
            get
            {
                return extension;
            }
        }

        public string HtmlFile
        {
            get
            {
                return htmlFile;
            }
        }

        public String FileNoExtension
        {
            get
            {
                return fileNoExtension;
            }
        }

        public bool IsProjectFile
        {
            get
            {
                return htmlFile.StartsWith("edity/", StringComparison.OrdinalIgnoreCase);
            }
        }

        public TemplateEnvironment Environment
        {
            get
            {
                return environment;
            }
        }

        /// <summary>
        /// This will be true if the current path can point to a new html file.
        /// </summary>
        public bool PathCanCreateFile
        {
            get
            {
                return extension == "" && !File.Exists(htmlFile) && !Directory.Exists(directory);
            }
        }

        /// <summary>
        /// Set this to true to skip the file defined by HtmlFile when building the page. This
        /// is useful to load pages like a new page that does not have actual content yet.
        /// </summary>
        public bool SkipHtmlFile { get; set; } = false;

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

        /// <summary>
        /// Load the page stack. The pages will be loaded and returned from innermost
        /// to outermost.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageStackItem> loadPageStack()
        {
            PageStackItem page;
            if (!SkipHtmlFile)
            {
                using (var source = new StreamReader(File.OpenRead(getFullRealPath(htmlFile))))
                {
                    page = new PageStackItem()
                    {
                        Content = source.ReadToEnd(),
                        PageDefinition = getPageDefinition(htmlFile),
                        PageScriptPath = getPageFile(htmlFile, htmlFile, "js"),
                        PageCssPath = getPageFile(htmlFile, htmlFile, "css"),
                    };
                }
                yield return page;
            }
            for(int i = templates.Count - 1; i >= 0; --i)
            {
                var template = templates[i];
                var realTemplate = findRealFile(template);
                using (var layout = new StreamReader(File.OpenRead(realTemplate)))
                {
                    page = new PageStackItem()
                    {
                        Content = layout.ReadToEnd(),
                        PageDefinition = getPageDefinition(realTemplate),
                        PageScriptPath = getPageFile(realTemplate, template, "js"),
                        PageCssPath = getPageFile(realTemplate, template, "css"),
                    };
                }
                yield return page;
            }
        }

        /// <summary>
        /// Find the full real file path if the file exists, if not returns the original
        /// file name.
        /// </summary>
        /// <param name="file">The file to look for.</param>
        /// <returns>The full path to the real file, searching all dirs.</returns>
        public String findRealFile(String file)
        {
            bool usedBackup;
            return findRealFile(file, out usedBackup);
        }

        /// <summary>
        /// Find the full real file path if the file exists, if not returns the original
        /// </summary>
        /// <param name="file">The file to look for.</param>
        /// <param name="usedBackup">Will be set to true if the backup file was used.</param>
        /// <returns></returns>
        public String findRealFile(String file, out bool usedBackup)
        {
            usedBackup = false;

            var realFile = getFullRealPath(file);
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

            //Not found, just return the file
            return file;
        }

        private string getBackupPath(string file)
        {
            return Path.Combine(backupPath, file);
        }

        public string getEditorFile(String layoutName)
        {
            //returnFile
            String file = $"edity/editor/{layoutName}.html";
            return file;
        }

        public string getLayoutFile(String layoutName)
        {
            //returnFile
            String file = $"edity/layouts/{layoutName}.html";
            return file;
        }

        private static string detectIndexFile(string file)
        {
            if (file == null)
            {
                file = "index";
            }
            if (file.Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                file = "index.html";
            }

            return file;
        }

        private PageDefinition getPageDefinition(String file)
        {
            String settingsPath = Path.ChangeExtension(file, "json");
            PageDefinition pageSettings;
            if (System.IO.File.Exists(settingsPath))
            {
                using (var stream = new StreamReader(System.IO.File.Open(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
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
            if (System.IO.File.Exists(realPath))
            {
                return Path.ChangeExtension(linkFile, extension);
            }
            return null;
        }
    }
}
