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
            var controllerNode = HtmlRapierQueries.getControllerNode("treeMenu", document.DocumentNode);
            if(controllerNode != null)
            {
                controllerNode.SetAttributeValue("data-hr-config-treemenu-editmode", "true");
                controllerNode.AddClass("treemenu-editor");
                var rootControllerDoc = new HtmlDocument();
                rootControllerDoc.LoadHtml(rootEditor);
                controllerNode.InsertBefore(rootControllerDoc.DocumentNode, controllerNode.FirstChild);
                //Find templates
                var folderModel = HtmlRapierQueries.getModelNode("childItems", controllerNode);
                if (folderModel != null)
                {
                    var folderTemplate = HtmlRapierQueries.getModelTemplate(folderModel);
                    if (folderTemplate != null)
                    {
                        var firstElement = folderTemplate.FirstElementChild();
                        if (firstElement != null)
                        {
                            MakeDraggable(firstElement);
                        }

                        var insertNode = folderTemplate.Select("[data-hr-on-click='toggleMenuItem']").FirstOrDefault();
                        if (insertNode != null)
                        {
                            var childModel = HtmlRapierQueries.getModelNode("children", folderTemplate);
                            if(childModel != null)
                            {
                                var doc = new HtmlDocument();
                                doc.LoadHtml(folderEditor);
                                insertNode.AppendChild(doc.DocumentNode);
                                insertNode.AddClass("treemenu-editor-folder");
                            }
                        }
                    }

                    var linkTemplate = HtmlRapierQueries.getModelTemplate(folderModel, "link");
                    if (linkTemplate != null)
                    {
                        var insertNode = linkTemplate.FirstElementChild();
                        if (insertNode != null)
                        {
                            insertNode.InnerHtml += linkEditor;
                            insertNode.AddClass("treemenu-editor-link");
                            MakeDraggable(insertNode);
                        }
                    }
                }
            }
        }

        private static void MakeDraggable(HtmlNode insertNode)
        {
            insertNode.Attributes.Add("draggable", "true");
            insertNode.Attributes.Add("data-hr-on-dragstart", "dragStart");
            insertNode.Attributes.Add("data-hr-on-dragover", "dragOver");
            insertNode.Attributes.Add("data-hr-on-dragleave", "dragLeave");
            insertNode.Attributes.Add("data-hr-on-drop", "drop");
            insertNode.Attributes.Add("data-hr-class-above", "drop-above");
            insertNode.Attributes.Add("data-hr-class-inside", "drop-inside");
            insertNode.Attributes.Add("data-hr-class-below", "drop-below");
            insertNode.Attributes.Add("data-hr-toggle", "drag");
        }

        private static String rootEditor =
@"<div class=""treemenu-editor-rootcontrols"" data-hr-controller=""treeMenuEditRoot"" data-hr-model=""treeMenuEditRoot"" data-hr-keep>
        <a href = ""#"" data-hr-on-click=""addItem"" >&#x2b;</a>
    </div>";

        private static String folderEditor =
@"<div class=""treemenu-editor"">
    <a href=""#"" data-hr-on-click=""moveUp"">&nbsp;&#x2191;&nbsp;</a>
    <a href=""#"" data-hr-on-click=""moveDown"">&nbsp;&#x2193;&nbsp;</a>
    <a href=""#"" data-hr-on-click=""moveToParent"">&#x2190;</a>
    <a href=""#"" data-hr-on-click=""moveToChild"">&#x2192;</a>
    <a href=""#"" data-hr-on-click=""addItem"">&#x2b;</a>
    <a href=""#"" data-hr-on-click=""editItem"">&#x270f;</a>
    <a href=""#"" data-hr-on-click=""deleteItem"">&#x1f5d1;</a>
</div>";

        private static String linkEditor =
@"<div class=""treemenu-editor"">
    <a href=""#"" data-hr-on-click=""moveUp"">&#x2191;</a>
    <a href=""#"" data-hr-on-click=""moveDown"">&#x2193;</a>
    <a href=""#"" data-hr-on-click=""moveToParent"">&#x2190;</a>
    <a href=""#"" data-hr-on-click=""moveToChild"">&#x2192;</a>
    <a href=""#"" data-hr-on-click=""editItem"">&#x270f;</a>
    <a href=""#"" data-hr-on-click=""deleteItem"">&#x1f5d1;</a>
</div>";
    }
}
