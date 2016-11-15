"use strict";


import * as domQuery from "hr.domquery";
import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as navmenu from "hr.widgets.navmenu";
import * as pageService from "edity.editorcore.PageService";

var CodeMirror = (<any>window).CodeMirror;

class EditSourceController {
    private editSourceDialog;
    private cm;

    constructor(bindings: controller.BindingCollection) {
        this.editSourceDialog = bindings.getToggle('dialog');
        var codemirrorElement = domQuery.first('#editSourceTextarea');
        this.cm = CodeMirror.fromTextArea(codemirrorElement, {
            lineNumbers: true,
            mode: "htmlmixed",
            theme: "edity"
        });
    }

    apply(evt) {
        evt.preventDefault();
        this.editSourceDialog.off();
        pageService.setHtml(this.cm.getValue());
    }

    startEdit() {
        this.editSourceDialog.on();
        this.cm.setSize(null, window.innerHeight - 250);
        this.cm.setValue(pageService.getHtml());
        setTimeout(() => this.cm.refresh(), 500);
    }
}

var editSourceController = controller.create<EditSourceController, void, void>("editSource", EditSourceController)[0];

class NavItemController {
    edit() {
        editSourceController.startEdit();
    }
}

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.add("EditSourceNavItem", NavItemController);