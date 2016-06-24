﻿using EdityMcEditface.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCompiler
{
    public class JsonCompiler : ContentCompiler
    {
        private String inDir;
        private String outDir;
        private String backupPath;
        private String layout;

        public JsonCompiler(String inDir, String outDir, String backupPath, String layout)
        {
            this.inDir = inDir;
            this.outDir = outDir;
            this.backupPath = backupPath;
            this.layout = layout;
        }

        public void buildPage(string relativeFile)
        {
            var inFile = Path.Combine(inDir, relativeFile);
            var outFile = Path.Combine(this.outDir, relativeFile);

            var extension = "json";
            if (OutputExtension != null)
            {
                extension = OutputExtension;
            }
            outFile = Path.ChangeExtension(outFile, extension);

            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            TargetFileInfo fileInfo = new TargetFileInfo(relativeFile);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder.Project);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            pageStack.pushLayout(layout);

            var dr = new PlainTextRenderer(environment, '<', '>');
            var document = dr.getDocument(pageStack.Pages);
            var outDir = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            using (var writer = new StreamWriter(File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(document);
            }
        }

        public void copyProjectContent()
        {
            
        }

        /// <summary>
        /// Change the extension for the output file, by default the extension is .json.
        /// </summary>
        public String OutputExtension { get; set; } = null;
    }
}