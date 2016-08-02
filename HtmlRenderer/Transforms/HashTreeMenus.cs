using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Security.Cryptography;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class HashTreeMenus : ServerSideTransform
    {
        FileFinder fileFinder;

        public HashTreeMenus(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        public void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions)
        {
            using (var sha = new SHA256Managed())
            {
                var controllerNode = HtmlrestQueries.getControllerNode("treeMenu", document.DocumentNode);
                if (controllerNode != null)
                {
                    var file = HtmlrestQueries.getModelSrc(controllerNode);
                    if (file != null)
                    {
                        using (var stream = fileFinder.readFile(file))
                        {
                            byte[] checksum = sha.ComputeHash(stream);
                            var hash = BitConverter.ToString(checksum).Replace("-", String.Empty);
                            controllerNode.SetAttributeValue("data-hr-config-treemenu-version", hash);
                        }
                    }
                }
            }
        }
    }
}
