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
            fileFinder.pushLayout(defaultTemplate);
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
            fileFinder.copyDependencyFiles(outDir);
        }

        public void copyProjectContent()
        {
            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            fileFinder.useFile("index");

            fileFinder.copyProjectContent(outDir);
        }

        /// <summary>
        /// Change the extension for the output file, by default the extension is not changed.
        /// </summary>
        public String OutputExtension { get; set; } = null;
    }
}
