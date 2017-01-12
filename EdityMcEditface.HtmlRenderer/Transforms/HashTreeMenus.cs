using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Security.Cryptography;
using System.IO;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class HashTreeMenus : ServerSideTransform
    {
        IFileFinder fileFinder;

        public HashTreeMenus(IFileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        public void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions)
        {
            using (var sha = new SHA256Managed())
            {
                var controllerNode = HtmlRapierQueries.getControllerNode("treeMenu", document.DocumentNode);
                if (controllerNode != null)
                {
                    var file = HtmlRapierQueries.getModelSrc(controllerNode);
                    if (file != null)
                    {
                        try
                        {
                            using (var stream = fileFinder.readFile(file))
                            {
                                byte[] checksum = sha.ComputeHash(stream);
                                var hash = BitConverter.ToString(checksum).Replace("-", String.Empty);
                                controllerNode.SetAttributeValue("data-hr-config-treemenu-version", hash);
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            //Ignored, just means we don't write a hash
                        }
                    }
                }
            }
        }
    }
}
