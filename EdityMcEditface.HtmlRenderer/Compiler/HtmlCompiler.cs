using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
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
        private ITargetFileInfoProvider fileInfoProvider;
        private String version;
        private IOverrideValuesProvider overrideValuesProvider;

        public HtmlCompiler(IFileFinder fileFinder, String outDir, ITargetFileInfoProvider fileInfoProvider, IOverrideValuesProvider overrideValuesProvider)
        {
            this.fileFinder = fileFinder;
            this.outDir = outDir;
            this.fileInfoProvider = fileInfoProvider;
            this.version = DateTime.Now.ToString("yyyyMMddhhmmss");
            this.overrideValuesProvider = overrideValuesProvider;
        }

        public void buildPage(String relativeFile)
        {
            relativeFile = relativeFile.TrimStartingPathChars();

            var outFile = Path.Combine(this.outDir, relativeFile);

            if(OutputExtension != null)
            {
                outFile = Path.ChangeExtension(outFile, OutputExtension);
            }

            ITargetFileInfo fileInfo = fileInfoProvider.GetFileInfo(relativeFile, null);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder, overrideValuesProvider.OverrideVars, version, true);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            if (pageStack.Visible)
            {
                var pageSettings = fileFinder.GetProjectPageDefinition(fileInfo);
                var layout = pageSettings.Layout;
                if(OutputExtension != null)
                {
                    layout = Path.ChangeExtension(layout, OutputExtension);
                }
                pageStack.pushLayout(layout);

                var pathBase = environment.PathBase;

                HtmlDocumentRenderer dr = new HtmlDocumentRenderer(environment);
                dr.addTransform(new HashTreeMenus(fileFinder));
                dr.addTransform(new ExpandRootedPaths(pathBase));
                if (!String.IsNullOrEmpty(pathBase))
                {
                    //Safe to fix relative urls last since it wont replace any that already start with the path base.
                    dr.addTransform(new FixRelativeUrls(pathBase));
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
                fileFinder.CopyLinkedFiles(this.outDir, pageStack);
            }
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
