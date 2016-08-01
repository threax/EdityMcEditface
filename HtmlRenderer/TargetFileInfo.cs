using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class TargetFileInfo
    {
        private String originalFileName;
        private String extension;
        private String fileNoExtension;
        private String htmlFile;
        private String directory;
        private bool isDirectory = false;

        /// <summary>
        /// Use the specified file as the actual page. This must be called before
        /// most other functions will do anything useful.
        /// </summary>
        /// <param name="file"></param>
        public TargetFileInfo(String file)
        {
            file = detectIndexFile(file);
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

        public bool IsProjectFile
        {
            get
            {
                return htmlFile.StartsWith("edity/", StringComparison.OrdinalIgnoreCase);
            }
        }

        public String FileNoExtension
        {
            get
            {
                return fileNoExtension;
            }
        }

        public String OriginalFileName
        {
            get
            {
                return originalFileName;
            }
        }

        public String NoHtmlRedirect
        {
            get
            {
                var path = Path.GetDirectoryName(originalFileName);
                var file = Path.GetFileNameWithoutExtension(originalFileName);
                if (String.IsNullOrEmpty(path))
                {
                    path = "/";
                }
                if ("index".Equals(file, StringComparison.InvariantCultureIgnoreCase))
                {
                    file = "";
                }
                return Path.Combine(path, file);
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

        private static string detectIndexFile(string file)
        {
            if (file == null)
            {
                file = "index";
            }
            file = file.TrimStart('\\', '/');
            if (file.Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                file = "index.html";
            }

            return file;
        }
    }
}
