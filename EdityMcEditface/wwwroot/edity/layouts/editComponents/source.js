"use strict";

jsns.run([
    "htmlrest.domquery",
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "edity.pageSourceSync"
],
function (exports, module, domQuery, storage, rest, controller, navmenu, sourceSync) {

    function EditSourceController(bindings) {
        var editSourceDialog = bindings.getToggle('dialog');
        var codemirrorElement = domQuery.first('#editSourceTextarea');
        var cm = CodeMirror.fromTextArea(codemirrorElement, {
            lineNumbers: true,
            mode: "htmlmixed"
        });

        function apply(evt) {
            evt.preventDefault();
            editSourceDialog.off();
            sourceSync.setHtml(cm.getValue());
        }
        this.apply = apply;

        function NavItemController() {
            function edit() {
                editSourceDialog.on();
                cm.setValue(sourceSync.getHtml());
                setTimeout(function () {
                    cm.refresh();
                }, 500);
            }
            this.edit = edit;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("EditSourceNavItem", NavItemController);
    }

    controller.create("editSource", EditSourceController);

});