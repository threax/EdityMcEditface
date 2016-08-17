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
            var controllerNode = HtmlrestQueries.getControllerNode("treeMenu", document.DocumentNode);
            if(controllerNode != null)
            {
                controllerNode.SetAttributeValue("data-hr-config-treemenu-editmode", "true");
                controllerNode.AddClass("treemenu-editor");
                var rootControllerDoc = new HtmlDocument();
                rootControllerDoc.LoadHtml(rootEditor);
                controllerNode.InsertBefore(rootControllerDoc.DocumentNode, controllerNode.FirstChild);
                //Find templates
                var folderModel = HtmlrestQueries.getModelNode("folders", controllerNode);
                if (folderModel != null)
                {
                    var folderTemplate = HtmlrestQueries.getModelTemplate(folderModel);
                    if (folderTemplate != null)
                    {
                        var insertNode = folderTemplate.Select("[data-hr-on-click='toggleMenuItem']").FirstOrDefault();
                        if (insertNode != null)
                        {
                            var childModel = HtmlrestQueries.getModelNode("children", folderTemplate);
                            if(childModel != null)
                            {
                                var doc = new HtmlDocument();
                                doc.LoadHtml(folderEditor);
                                insertNode.AppendChild(doc.DocumentNode);
                                insertNode.AddClass("treemenu-editor-folder");
                            }
                        }
                    }
                }

                var linkModel = HtmlrestQueries.getModelNode("links", controllerNode);
                if (linkModel != null)
                {
                    var linkTemplate = HtmlrestQueries.getModelTemplate(linkModel);
                    if (linkTemplate != null)
                    {
                        var insertNode = linkTemplate.FirstElementChild();
                        if (insertNode != null)
                        {
                            insertNode.InnerHtml += linkEditor;
                            insertNode.AddClass("treemenu-editor-link");
                        }
                    }
                }
            }
        }

        private static String rootEditor =
@"<div class=""treemenu-editor-rootcontrols"" data-hr-controller=""treeMenuEditRoot"" data-hr-model=""treeMenuEditRoot"" data-hr-keep>
        <a href = ""#"" data-hr-on-click=""save"" ><span class=""glyphicon glyphicon-floppy-disk"" ></span></a>
        <a href = ""#"" data-hr-on-click=""addItem"" ><span class=""glyphicon glyphicon-plus"" ></span></a>
        <div class=""load-linebar center-block pull-right"" data-hr-toggle=""loading"" data-hr-style-off=""display:none"">
            <div class=""rect1""></div>
            <div class=""rect2""></div>
            <div class=""rect3""></div>
            <div class=""rect4""></div>
            <div class=""rect5""></div>
        </div>
    </div>";

        private static String folderEditor =
@"<div class=""treemenu-editor"">
    <a href=""#"" data-hr-on-click=""moveUp""><span class=""glyphicon glyphicon-arrow-up""></span></a>
    <a href=""#"" data-hr-on-click=""moveDown""><span class=""glyphicon glyphicon-arrow-down""></span></a>
    <a href=""#"" data-hr-on-click=""moveToParent""><span class=""glyphicon glyphicon-arrow-left""></span></a>
    <a href=""#"" data-hr-on-click=""moveToChild""><span class=""glyphicon glyphicon-arrow-right""></span></a>
    <a href=""#"" data-hr-on-click=""addItem""><span class=""glyphicon glyphicon-plus"" ></span></a>
    <a href=""#"" data-hr-on-click=""editItem""><span class=""glyphicon glyphicon-pencil"" ></span></a>
    <a href=""#"" data-hr-on-click=""deleteItem""><span class=""glyphicon glyphicon-trash"" ></span></a>
</div>";

        private static String linkEditor =
@"<div class=""treemenu-editor"">
    <a href=""#"" data-hr-on-click=""moveUp""><span class=""glyphicon glyphicon-arrow-up""></span></a>
    <a href=""#"" data-hr-on-click=""moveDown""><span class=""glyphicon glyphicon-arrow-down""></span></a>
    <a href=""#"" data-hr-on-click=""moveToParent""><span class=""glyphicon glyphicon-arrow-left""></span></a>
    <a href=""#"" data-hr-on-click=""moveToChild""><span class=""glyphicon glyphicon-arrow-right""></span></a>
    <a href=""#"" data-hr-on-click=""editItem""><span class=""glyphicon glyphicon-pencil"" ></span></a>
    <a href=""#"" data-hr-on-click=""deleteItem""><span class=""glyphicon glyphicon-trash"" ></span></a>
</div>";
    }
}
