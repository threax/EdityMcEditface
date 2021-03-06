﻿using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class JsonCompiler : IContentCompiler
    {
        private IFileFinder fileFinder;
        private String outDir;
        private ITargetFileInfoProvider fileInfoProvider;
        private IOverrideValuesProvider overrideValuesProvider;

        public JsonCompiler(IFileFinder fileFinder, String outDir, ITargetFileInfoProvider fileInfoProvider, IOverrideValuesProvider overrideValuesProvider)
        {
            this.fileFinder = fileFinder;
            this.outDir = outDir;
            this.fileInfoProvider = fileInfoProvider;
            this.overrideValuesProvider = overrideValuesProvider;
        }

        public void buildPage(string relativeFile)
        {
            relativeFile = relativeFile.TrimStartingPathChars();

            var outFile = Path.Combine(this.outDir, relativeFile);

            var extension = "json";
            if (OutputExtension != null)
            {
                extension = OutputExtension;
            }
            outFile = Path.ChangeExtension(outFile, extension);

            var fileInfo = fileInfoProvider.GetFileInfo(relativeFile, null);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder, overrideValuesProvider.OverrideVars, null, true);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            if (pageStack.Visible)
            {
                pageStack.ContentTransformer = (content) =>
                {
                //This removes all html elements and formatting and cleans up the whitespace
                HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(content);
                    var escaped = HtmlEntity.DeEntitize(htmlDoc.DocumentNode.InnerText);
                    escaped = escaped.SingleSpaceWhitespace();
                    return escaped.JsonEscape();
                };
                var pageSettings = fileFinder.GetProjectPageDefinition(fileInfo);
                var layout = pageSettings.Layout;
                if (OutputExtension != null)
                {
                    layout = Path.ChangeExtension(layout, OutputExtension);
                }
                pageStack.pushLayout(layout);

                var dr = new PlainTextRenderer(environment, StringExtensions.JsonEscape);
                var document = dr.getDocument(pageStack.Pages);
                var outDir = Path.GetDirectoryName(outFile);
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }
                using (var writer = new StreamWriter(File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8))
                {
                    writer.Write(document);
                }
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
