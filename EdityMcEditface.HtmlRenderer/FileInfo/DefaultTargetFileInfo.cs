using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.FileInfo
{
    public class DefaultTargetFileInfo : ITargetFileInfo
    {
        private String defaultFileName;
        private String originalFileName;
        private String extension;
        private String fileNoExtension;
        private String htmlFile;
        private String directory;
        private bool isDirectory = false;
        private String pathBase;

        /// <summary>
        /// Use the specified file as the actual page. This must be called before
        /// most other functions will do anything useful.
        /// </summary>
        /// <param name="file">The file name to lookup.</param>
        /// <param name="pathBase">The path base, will be removed from the file name if not null.</param>
        /// <param name="defaultFileName">The name of the default document for this site, normally would be something like index.html.</param>
        public DefaultTargetFileInfo(String file, String pathBase, String defaultFileName)
        {
            this.defaultFileName = defaultFileName;
            this.pathBase = pathBase;

            //Ensure leading slash
            if(file != null)
            {
                file = file.EnsureStartingPathSlash();
            }

            if(pathBase != null)
            {
                pathBase.EnsureStartingPathSlash();
            }

            //Remove path base
            file = RemovePathBase(file, pathBase);

            file = detectIndexFile(file, defaultFileName);
            originalFileName = file;

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
        }

        /// <summary>
        /// The extension of the file.
        /// </summary>
        public string Extension
        {
            get
            {
                return extension;
            }
        }

        /// <summary>
        /// The file as an html file, with the .html extension. DerivedFileName is probably more useful.
        /// </summary>
        public string HtmlFile
        {
            get
            {
                return htmlFile;
            }
        }

        /// <summary>
        /// This will be true if the target file is an html file.
        /// </summary>
        public bool PointsToHtmlFile
        {
            get
            {
                return !isDirectory && (extension == "" || ".html".Equals(extension, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// The derived file name for the file on the disk. 
        /// If this is an html file it will have the correct name and extension, otherwise it will return the OriginalFileName.
        /// This will correctly handle index.html files and files with no extensions.
        /// </summary>
        public String DerivedFileName
        {
            get
            {
                if (PointsToHtmlFile)
                {
                    return htmlFile;
                }
                return originalFileName;
            }
        }

        /// <summary>
        /// Return true if the file is a project file.
        /// </summary>
        public bool IsProjectFile
        {
            get
            {
                return htmlFile.StartsWith("edity/", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Get the name of the file with no extension.
        /// </summary>
        public String FileNoExtension
        {
            get
            {
                return fileNoExtension;
            }
        }

        /// <summary>
        /// The file name as it was originally seen
        /// </summary>
        public String OriginalFileName
        {
            get
            {
                return originalFileName;
            }
        }

        /// <summary>
        /// Redirect to the file without an html extension, unlike most methods here
        /// this will also include the original path base.
        /// </summary>
        public String NoHtmlRedirect
        {
            get
            {
                var path = Path.GetDirectoryName(originalFileName);
                var file = Path.GetFileNameWithoutExtension(originalFileName);
                var pathBase = this.pathBase;
                if(String.IsNullOrEmpty(pathBase))
                {
                    pathBase = "/";
                }
                if (defaultFileName.Equals(file, StringComparison.OrdinalIgnoreCase))
                {
                    file = "";
                }
                return  Path.Combine(pathBase, path, file);
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

        private static string detectIndexFile(string file, string defaultFileName)
        {
            if (file == null)
            {
                file = defaultFileName;
            }
            file = file.TrimStart('\\', '/');
            if(file == "" || file.Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                file = defaultFileName;
            }

            return file;
        }

        public static string RemovePathBase(string path, string pathBase)
        {
            if (pathBase != null && path != null && path.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(pathBase.Length);
            }

            return path;
        }
    }
}
