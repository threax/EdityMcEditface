using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using EdityMcEditface.HtmlRenderer;

namespace SiteCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddCommandLine(args);
            Configuration = builder.Build();

            InDir = Configuration["inDir"];
            OutDir = Configuration["outDir"];
            BackupPath = Configuration["backupPath"];
            var edityDir = "edity";

            bool noInputMode = Configuration["noInput"] != null;

            //Handle output folder
            if (Directory.Exists(OutDir))
            {
                bool deleteDir = noInputMode;
                if (!noInputMode)
                {
                    Console.WriteLine($"Output directory {OutDir} already exists, do you want to delete it?");
                    deleteDir = Console.ReadKey().Key == ConsoleKey.Y;
                }
                if (deleteDir)
                {
                    Directory.Delete(OutDir, true);
                }
                else
                {
                    Console.WriteLine("Cannot write website to a folder with files in it, shutting down.");
                }
            }

            Directory.CreateDirectory(OutDir);

            foreach (var file in Directory.EnumerateFiles(InDir, "*.html", SearchOption.AllDirectories))
            {
                var relativeFile = FileFinder.TrimStartingPathChars(file.Substring(InDir.Length));
                if (!relativeFile.StartsWith(edityDir, StringComparison.OrdinalIgnoreCase))
                {
                    buildPage(Path.Combine(InDir, relativeFile), Path.Combine(OutDir, relativeFile));
                }
            }

            FileFinder fileFinder = new FileFinder(InDir, BackupPath);
            fileFinder.useFile("index");

            foreach(var file in fileFinder.Project.AdditionalContent)
            {
                var realFile = fileFinder.findRealFile(file);
                if (File.Exists(realFile))
                {
                    copyFileIfNotExists(realFile, safePathCombine(OutDir, file));
                }
                else
                {
                    //Copy all files from normal and backup location
                    copyFolderContents(fileFinder.getFullRealPath(file), file);
                    copyFolderContents(fileFinder.getBackupPath(file), file);
                }
            }

            Console.WriteLine($"All files written to {OutDir}");
            Console.ReadKey();
        }

        private static void copyFolderContents(string sourceDir, String additionalPath)
        {
            if (Directory.Exists(sourceDir))
            {
                foreach (var dirFile in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    var relativePath = dirFile.Substring(sourceDir.Length);
                    copyFileIfNotExists(dirFile, safePathCombine(OutDir, additionalPath, relativePath));
                }
            }
        }

        public static void buildPage(String inFile, String outFile)
        {
            FileFinder fileFinder = new FileFinder(InDir, BackupPath);
            fileFinder.useFile(inFile);
            fileFinder.pushTemplate(fileFinder.getLayoutFile("default"));
            DocumentRenderer dr = new DocumentRenderer(fileFinder.Environment);
            var document = dr.getDocument(fileFinder.loadPageStack());
            var outDir = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            using(var writer = new StreamWriter(File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(document.DocumentNode.OuterHtml);
            }
            copyDependencyFiles(fileFinder);
            Console.WriteLine($"{inFile} to {outFile}");
        }

        private static HashSet<String> copiedContentFiles = new HashSet<string>();

        public static void copyDependencyFiles(FileFinder fileFinder)
        {
            foreach(var page in fileFinder.loadPageStack())
            {
                if (!String.IsNullOrEmpty(page.PageCssPath))
                {
                    copyFileIfNotExists(safePathCombine(InDir, page.PageCssPath), safePathCombine(OutDir, page.PageCssPath));
                }
                if (!String.IsNullOrEmpty(page.PageScriptPath))
                {
                    copyFileIfNotExists(safePathCombine(InDir, page.PageScriptPath), safePathCombine(OutDir, page.PageScriptPath));
                }
                foreach(var content in fileFinder.LinkedContentFiles)
                {
                    if (!copiedContentFiles.Contains(content) && isValidPhysicalFile(content))
                    {
                        copyFileIfNotExists(fileFinder.findRealFile(content), safePathCombine(OutDir, content));
                    }
                    copiedContentFiles.Add(content);
                }
            }
        }

        public static bool isValidPhysicalFile(String file)
        {
            try
            {
                Path.GetFullPath(file);
            }
            catch(NotSupportedException)
            {
                return false;
            }
            return true;
        }

        public static String safePathCombine(params string[] paths)
        {
            for(int i = 1; i < paths.Length; ++i)
            {
                paths[i] = FileFinder.TrimStartingPathChars(paths[i]);
            }
            return Path.Combine(paths);
        }

        public static void copyFileIfNotExists(String source, String dest)
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

        public static IConfigurationRoot Configuration { get; private set; }

        public static String InDir { get; set; }

        public static String OutDir { get; set; }

        public static String BackupPath { get; set; }
    }
}
