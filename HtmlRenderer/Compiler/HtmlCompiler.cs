using EdityMcEditface.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class HtmlCompiler : ContentCompiler
    {
        private String inDir;
        private String outDir;
        private String backupPath;
        private String layout;

        public HtmlCompiler(String inDir, String outDir, String backupPath, String layout)
        {
            this.inDir = inDir;
            this.outDir = outDir;
            this.backupPath = backupPath;
            this.layout = layout;
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
            TargetFileInfo fileInfo = new TargetFileInfo(relativeFile);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder.Project);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            pageStack.pushLayout(layout);

            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(environment);
            var document = dr.getDocument(pageStack.Pages);
            var outDir = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            using (var writer = new StreamWriter(File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(document.DocumentNode.OuterHtml);
            }
            fileFinder.copyDependencyFiles(outDir, pageStack);
        }

        public void copyProjectContent()
        {
            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            fileFinder.copyProjectContent(outDir);
        }

        /// <summary>
        /// Change the extension for the output file, by default the extension is not changed.
        /// </summary>
        public String OutputExtension { get; set; } = null;
    }
}
