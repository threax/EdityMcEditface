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
        private String template;

        public PdfCompiler(String inDir, String outDir, String backupPath, String template)
        {
            this.inDir = inDir;
            this.outDir = outDir;
            this.backupPath = backupPath;
            this.template = template;
        }

        public void buildPage(string relativeFile)
        {
            relativeFile = Path.ChangeExtension(relativeFile, template);

            FileFinder fileFinder = new FileFinder(inDir, backupPath);
            fileFinder.useFile(relativeFile);

            HtmlDocument document = new HtmlDocument();
            var inputFile = fileFinder.getFullRealPath(relativeFile);
            using (var stream = File.Open(inputFile, FileMode.Open, FileAccess.Read))
            {
                document.Load(stream);
            }
            var images = document.DocumentNode.Select("img[src]");
            foreach(var image in images)
            {
                fixLink(image, "src");
            }
            var stylesheets = document.DocumentNode.Select("link[href]");
            foreach(var style in stylesheets)
            {
                fixLink(style, "href");
            }

            using(var stream = File.Open(inputFile, FileMode.Create, FileAccess.Write))
            {
                document.Save(stream);
            }

            //ProcessStartInfo pi = new ProcessStartInfo()
            //{
            //    Arguments = $"{inputFile} {}",
            //};
            //Process.Start(pi);
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
                    node.SetAttributeValue(attribute, Path.Combine(outDir, val.TrimStart('\\', '/')));
                    break;
            }
        }
    }
}
