using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class PublishedFileDetector : IPublishedFileDetector
    {
        public bool IsPublishableFile(string originalFile, string normalizedFile)
        {
            var htmlFile = Path.ChangeExtension(normalizedFile, "html");
            return File.Exists(htmlFile);
        }
    }
}
