using EdityMcEditface.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCompiler
{
    public class HtmlCompiler : ContentCompiler
    {
        private String inDir;
        private String outDir;
        private String backupPath;
        private String defaultTemplate;

        public HtmlCompiler(String inDir, String outDir, String backupPath, String defaultTemplate)
        {
            this.inDir = inDir;
            this.outDir = outDir;
            this.backupPath = backupPath;
            this.defaultTemplate = defaultTemplate;
        }

        public void buildPage(String relativeFile)
        {
            var inFile = Path.Combine(inDir, relativeFile);
            var outFile = Path.Combine(this.outDir, relativeFile);

            if(OutputExtension != null)
            {
                outFile = Path.ChangeExtension(outFile, OutputExtension);
            }

            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            fileFinder.useFile(relativeFile);
            fileFinder.pushTemplate(fileFinder.getLayoutFile(defaultTemplate));
            DocumentRenderer dr = new DocumentRenderer(fileFinder.Environment);
            var document = dr.getDocument(fileFinder.loadPageStack());
            var outDir = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            using (var writer = new StreamWriter(File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(document.DocumentNode.OuterHtml);
            }
            copyDependencyFiles(fileFinder);
        }

        public void copyProjectContent()
        {
            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            fileFinder.useFile("index");

            foreach (var file in fileFinder.Project.AdditionalContent)
            {
                var realFile = fileFinder.findRealFile(file);
                if (File.Exists(realFile))
                {
                    copyFileIfNotExists(realFile, safePathCombine(outDir, file));
                }
                else
                {
                    //Copy all files from normal and backup location
                    copyFolderContents(fileFinder.getFullRealPath(file), file);
                    copyFolderContents(fileFinder.getBackupPath(file), file);
                }
            }
        }

        /// <summary>
        /// Change the extension for the output file, by default the extension is not changed.
        /// </summary>
        public String OutputExtension { get; set; } = null;

        private HashSet<String> copiedContentFiles = new HashSet<string>();

        private void copyDependencyFiles(FileFinder fileFinder)
        {
            foreach (var page in fileFinder.loadPageStack())
            {
                if (!String.IsNullOrEmpty(page.PageCssPath))
                {
                    copyFileIfNotExists(safePathCombine(inDir, page.PageCssPath), safePathCombine(outDir, page.PageCssPath));
                }
                if (!String.IsNullOrEmpty(page.PageScriptPath))
                {
                    copyFileIfNotExists(safePathCombine(inDir, page.PageScriptPath), safePathCombine(outDir, page.PageScriptPath));
                }
                foreach (var content in fileFinder.LinkedContentFiles)
                {
                    if (!copiedContentFiles.Contains(content) && isValidPhysicalFile(content))
                    {
                        copyFileIfNotExists(fileFinder.findRealFile(content), safePathCombine(outDir, content));
                    }
                    copiedContentFiles.Add(content);
                }
            }
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

        private void copyFolderContents(string sourceDir, String additionalPath)
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
    }
}
