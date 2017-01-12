using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class HtmlCompiler : IContentCompiler
    {
        private IFileFinder fileFinder;
        private String outDir;
        private String layout;
        private String siteRoot;

        public HtmlCompiler(IFileFinder fileFinder, String outDir, String layout, Dictionary<String, String> settings)
        {
            this.fileFinder = fileFinder;
            this.outDir = outDir;
            this.layout = layout;
            settings.TryGetValue("siteRoot", out this.siteRoot);
        }

        public void buildPage(String relativeFile)
        {
            var outFile = Path.Combine(this.outDir, relativeFile);

            if(OutputExtension != null)
            {
                outFile = Path.ChangeExtension(outFile, OutputExtension);
            }

            TargetFileInfo fileInfo = new TargetFileInfo(relativeFile);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder.Project);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            pageStack.pushLayout(layout);

            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(environment);
            dr.addTransform(new HashTreeMenus(fileFinder));
            if (!String.IsNullOrEmpty(siteRoot))
            {
                dr.addTransform(new FixRelativeUrls(siteRoot));
            }
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
            fileFinder.CopyDependencyFiles(this.outDir, pageStack);
        }

        public void copyProjectContent()
        {
            fileFinder.CopyProjectContent(outDir);
        }

        /// <summary>
        /// Change the extension for the output file, by default the extension is not changed.
        /// </summary>
        public String OutputExtension { get; set; } = null;
    }
}
