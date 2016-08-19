"use strict";

jsns.run([
    "hr.domquery",
    "hr.storage",
    "hr.controller",
    "hr.widgets.navmenu",
    "edity.PageService"
],
function (exports, module, domQuery, storage, controller, navmenu, pageService) {

    function EditSourceController(bindings) {
        var editSourceDialog = bindings.getToggle('dialog');
        var codemirrorElement = domQuery.first('#editSourceTextarea');
        var cm = CodeMirror.fromTextArea(codemirrorElement, {
            lineNumbers: true,
            mode: "htmlmixed",
            theme: "edity"
        });

        function apply(evt) {
            evt.preventDefault();
            editSourceDialog.off();
            pageService.setHtml(cm.getValue());
        }
        this.apply = apply;

        function NavItemController() {
            function edit() {
                editSourceDialog.on();
                cm.setSize(null, window.innerHeight - 250);
                cm.setValue(pageService.getHtml());
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