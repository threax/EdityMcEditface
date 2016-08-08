"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "edity.pageSourceSync"
],
function (exports, module, storage, rest, controller, navmenu, sourceSync) {

    function EditSourceController(bindings) {
        var sourceModel = bindings.getModel('source');
        var editSourceDialog = bindings.getToggle('dialog');

        function apply(evt) {
            evt.preventDefault();
            editSourceDialog.off();
            sourceSync.setHtml(sourceModel.getData().source);
        }
        this.apply = apply;

        function NavItemController() {
            function edit() {
                editSourceDialog.on();
                sourceModel.setData({
                    source: sourceSync.getHtml()
                });
            }
            this.edit = edit;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("EditSourceNavItem", NavItemController);
    }

    controller.create("editSource", EditSourceController);

});