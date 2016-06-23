using EdityMcEditface.HtmlRenderer;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCompiler
{
    public class PdfCompiler : ContentCompiler
    {
        private String inDir;
        private String outDir;
        private String backupPath;
        private String layout;

        public PdfCompiler(String inDir, String outDir, String backupPath, String layout)
        {
            this.inDir = inDir;
            this.outDir = outDir;
            this.backupPath = backupPath;
            this.layout = layout;
        }

        public void buildPage(string relativeFile)
        {
            var inFile = Path.Combine(inDir, relativeFile);
            var outBaseFile = Path.Combine(this.outDir, relativeFile);
            outBaseFile = Path.ChangeExtension(outBaseFile, null);
            var outTempFile = outBaseFile + "_temp.html";

            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            TargetFileInfo fileInfo = new TargetFileInfo(relativeFile);
            TemplateEnvironment environment = new TemplateEnvironment(fileInfo.FileNoExtension, fileFinder.Project);
            PageStack pageStack = new PageStack(environment, fileFinder);
            pageStack.ContentFile = fileInfo.HtmlFile;
            pageStack.pushLayout(layout);

            DocumentRenderer dr = new DocumentRenderer(environment);
            var document = dr.getDocument(pageStack.Pages);

            //Repair file for pdf
            var images = document.DocumentNode.Select("img[src]");
            if (images != null)
            {
                foreach (var image in images)
                {
                    fixLink(image, "src");
                }
            }
            var stylesheets = document.DocumentNode.Select("link[href]");
            if (stylesheets != null)
            {
                foreach (var style in stylesheets)
                {
                    fixLink(style, "href");
                }
            }

            //Write temp file
            var outDir = Path.GetDirectoryName(outTempFile);
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            using (var writer = new StreamWriter(File.Open(outTempFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write(document.DocumentNode.OuterHtml);
            }

            var pdfFile = Path.ChangeExtension(outBaseFile, ".pdf");

            ProcessStartInfo pi = new ProcessStartInfo()
            {
                FileName = "C:\\Program Files\\wkhtmltopdf\\bin\\wkhtmltopdf.exe",
                Arguments = $"{outTempFile} {pdfFile}",
            };
            var process = Process.Start(pi);
            process.WaitForExit();

            File.Delete(outTempFile);
        }

        public void copyProjectContent()
        {

        }

        private void fixLink(HtmlNode node, String attribute)
        {
            var val = node.GetAttributeValue(attribute, "");
            switch (val[0])
            {
                case '\\':
                case '/':
                    node.SetAttributeValue(attribute, "file:///" + Path.Combine(outDir, val.TrimStart('\\', '/')));
                    break;
            }
        }
    }
}
