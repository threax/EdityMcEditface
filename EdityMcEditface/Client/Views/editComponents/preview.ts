import * as navmenu from "edity.editorcore.navmenu";
import * as PageStart from 'edity.editorcore.EditorPageStart';

PageStart.init().then(config => {
    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("PreviewButton");
});