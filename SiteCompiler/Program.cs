using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using EdityMcEditface.HtmlRenderer;
using System.Diagnostics;
using EdityMcEditface.HtmlRenderer.Compiler;

namespace SiteCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddCommandLine(args);
            Configuration = builder.Build();

            var inDir = Configuration["inDir"];
            var outDir = Configuration["outDir"];
            var backupPath = Configuration["backupPath"];
            var edityDir = "edity";

            bool noInputMode = Configuration["noInput"] != null;

            //Handle output folder
            if (Directory.Exists(outDir))
            {
                bool deleteDir = noInputMode;
                if (!noInputMode)
                {
                    Console.WriteLine($"Output directory {outDir} already exists, do you want to delete it?");
                    deleteDir = Console.ReadKey().Key == ConsoleKey.Y;
                }
                if (deleteDir)
                {
                    Directory.Delete(outDir, true);
                }
                else
                {
                    Console.WriteLine("Cannot write website to a folder with files in it, shutting down.");
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Directory.CreateDirectory(outDir);

            var fileFinder = new FileFinder(inDir, backupPath);

            var compilers = ContentCompilerFactory.CreateCompilers(inDir, outDir, backupPath, fileFinder.Project.Compilers);

            foreach (var file in Directory.EnumerateFiles(inDir, "*.html", SearchOption.AllDirectories))
            {
                var relativeFile = FileFinder.TrimStartingPathChars(file.Substring(inDir.Length));
                if (!relativeFile.StartsWith(edityDir, StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var compiler in compilers)
                    {
                        compiler.buildPage(relativeFile);
                    }
                }
            }

            foreach (var compiler in compilers)
            {
                compiler.copyProjectContent();
            }

            sw.Stop();

            Console.WriteLine($"All files written to {outDir} in {sw.Elapsed.TotalSeconds} seconds");
            Console.ReadKey();
        }

        public static IConfigurationRoot Configuration { get; private set; }
    }
}
