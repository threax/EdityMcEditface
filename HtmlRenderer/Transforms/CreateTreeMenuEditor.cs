using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace EdityMcEditface.HtmlRenderer.Transforms
{
    public class CreateTreeMenuEditor : ServerSideTransform
    {
        public CreateTreeMenuEditor()
        {

        }

        public void transform(HtmlDocument document, TemplateEnvironment environment, List<PageStackItem> pageDefinitions)
        {
            var controllerNode = getControllerNode("treeMenu", document.DocumentNode);
            if(controllerNode != null)
            {
                controllerNode.SetAttributeValue("data-hr-config-treemenu-editmode", "true");
                //Find templates
                var folderModel = getModelNode("folders", controllerNode);
                if (folderModel != null)
                {
                    var folderTemplate = getModelTemplate(folderModel);
                    if (folderTemplate != null)
                    {
                        var insertNode = folderTemplate.Select("[data-hr-on-click='toggleMenuItem']").FirstOrDefault();
                        if (insertNode != null)
                        {
                            var childModel = getModelNode("children", folderTemplate);
                            if(childModel != null)
                            {
                                var doc = new HtmlDocument();
                                doc.LoadHtml(editorText);
                                insertNode.AppendChild(doc.DocumentNode);
                                addClass(insertNode, "treemenu-editor-folder");
                            }
                        }
                    }
                }

                var linkModel = getModelNode("links", controllerNode);
                if (linkModel != null)
                {
                    var linkTemplate = getModelTemplate(linkModel);
                    if (linkTemplate != null)
                    {
                        var insertNode = firstElementChild(linkTemplate);
                        if (insertNode != null)
                        {
                            insertNode.InnerHtml += editorText;
                            addClass(insertNode, "treemenu-editor-link");
                        }
                    }
                }
            }
        }

        private static HtmlNode getControllerNode(String name, HtmlNode context)
        {
            return context.Select($"[data-hr-controller='{name}']").FirstOrDefault();
        }

        private static HtmlNode getModelNode(String name, HtmlNode context)
        {
            return context.Select($"[data-hr-model='{name}']").FirstOrDefault();
        }

        private static HtmlNode getModelTemplate(HtmlNode modelNode)
        {
            var componentName = modelNode.GetAttributeValue("data-hr-model-component", (string)null);
            if(componentName != null)
            {
                return modelNode.OwnerDocument.DocumentNode.Select($"[data-hr-component='{componentName}']").FirstOrDefault();
            }
            else
            {
                return modelNode.Select("template").FirstOrDefault();
            }
        }

        private static void addClass(HtmlNode node, String cls)
        {
            var clses = node.GetAttributeValue("class", "");
            if(clses != "")
            {
                clses += " ";
            }
            clses += cls;
            node.SetAttributeValue("class", clses);
        }

        private static HtmlNode firstElementChild(HtmlNode node)
        {
            foreach(var child in node.ChildNodes)
            {
                if(child.NodeType == HtmlNodeType.Element)
                {
                    return child;
                }
            }
            return null;
        }

        private static String editorText =
@"<div class=""treemenu-editor"">
    <a href=""#"" data-hr-on-click=""moveUp""><span class=""glyphicon glyphicon-arrow-up""></span></a>
    <a href=""#"" data-hr-on-click=""moveDown""><span class=""glyphicon glyphicon-arrow-down""></span></a>
    <a href=""#"" data-hr-on-click=""addItem""><span class=""glyphicon glyphicon-plus"" ></span></a>
    <a href=""#"" data-hr-on-click=""editItem""><span class=""glyphicon glyphicon-pencil"" ></span></a>
    <a href=""#"" data-hr-on-click=""deleteItem""><span class=""glyphicon glyphicon-trash"" ></span></a>
</div>";
    }
}
