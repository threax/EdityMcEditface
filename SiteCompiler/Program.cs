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

            Console.WriteLine($"All files written to {OutDir}");
            Console.ReadKey();
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

        public static String safePathCombine(String a, String b)
        {
            b = FileFinder.TrimStartingPathChars(b);
            return Path.Combine(a, b);
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
