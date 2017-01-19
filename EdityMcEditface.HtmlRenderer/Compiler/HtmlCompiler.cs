﻿using EdityMcEditface.HtmlRenderer;
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
            if(!settings.TryGetValue("siteRoot", out this.siteRoot))
            {
                siteRoot = "";
            }
        }

        public void buildPage(String relativeFile)
        {
            relativeFile = relativeFile.TrimStartingPathChars();

            var outFile = Path.Combine(this.outDir, relativeFile);

            if(OutputExtension != null)
            {
                outFile = Path.ChangeExtension(outFile, OutputExtension);
            }

            TargetFileInfo fileInfo = new TargetFileInfo(relativeFile, null);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder.Project, siteRoot);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            pageStack.pushLayout(layout);

            HtmlDocumentRenderer dr = new HtmlDocumentRenderer(environment);
            dr.addTransform(new HashTreeMenus(fileFinder));
            dr.addTransform(new ExpandRootedPaths(siteRoot));
            if (!String.IsNullOrEmpty(siteRoot))
            {
                //Safe to fix relative urls last since it wont replace any that already start with the site root.
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
