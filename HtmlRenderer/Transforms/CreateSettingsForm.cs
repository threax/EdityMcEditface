using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class CreateSettingsForm : ServerSideTransform
    {
        private int stopEnumerationIndex;

        public CreateSettingsForm(int stopEnumerationIndex = int.MaxValue)
        {
            this.stopEnumerationIndex = stopEnumerationIndex;
        }

        public void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions)
        {
            var formNode = document.DocumentNode.Select("[data-settings-form]").FirstOrDefault();
            if(formNode != null)
            {   
                var controlNode = formNode.Select("[data-settings-form-control]").FirstOrDefault();
                if(controlNode != null)
                {
                    var valueProvider = new DictionaryValueProvider();
                    int index = 0;
                    foreach(var page in pageDefinitions)
                    {
                        if(index == stopEnumerationIndex)
                        {
                            break;
                        }

                        var definition = page.PageDefinition;
                        foreach(var variable in definition.Vars)
                        {
                            valueProvider.Values["name"] = variable.Key;
                            valueProvider.Values["prettyName"] = variable.Key;
                            valueProvider.Values["value"] = variable.Value;
                            var node = controlNode.CloneNode(true);
                            node.InnerHtml = TextFormatter.formatText(node.InnerHtml, valueProvider, WebUtility.HtmlEncode);
                            formNode.AppendChild(node);
                        }
                    }
                    controlNode.Remove();
                }
            }
        }
    }
}
