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
                if (!file.StartsWith(edityDir, StringComparison.OrdinalIgnoreCase))
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
            Console.WriteLine($"{inFile} to {outFile}");
        }

        public static IConfigurationRoot Configuration { get; private set; }

        public static String InDir { get; set; }

        public static String OutDir { get; set; }

        public static String BackupPath { get; set; }
    }
}
