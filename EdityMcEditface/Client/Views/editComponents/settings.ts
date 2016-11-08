"use strict";

import * as controller from "hr.controller";
import * as FormLifecycle from "clientlibs.FormLifecycle";
import * as storage from "hr.storage";
import { PageClient } from "clientlibs.EdityClient";
import * as navmenu from "hr.widgets.navmenu";

class DeletePageConfirmationController {
    private dialog;
    private model;
    private uploadUrl;
    private currentUrl;
    private client: PageClient;

    constructor(bindings) {
        this.dialog = bindings.getToggle('dialog');
        this.model = bindings.getModel("info");
        this.uploadUrl = this.model.getSrc();
        this.currentUrl;
        this.client = new PageClient();
    }

    deletePage(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        this.client.delete(this.currentUrl, null);
        this.currentUrl = null;
        this.dialog.off();
    }

    confirmDelete(name, url) {
        this.model.setData(name);
        this.currentUrl = url;
        this.dialog.on();
    }
}

class PageSettingsController {
    private formLifecycle;
    private dialog;

    constructor(bindings: controller.BindingCollection) {
        this.formLifecycle = new FormLifecycle.FormLifecycle(bindings);
        this.dialog = bindings.getToggle('dialog');
    }

    deletePage(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        deletePageConfirmation.confirmDelete(document.title, window.location.pathname);
        this.dialog.off();
    }

    open() {
        this.formLifecycle.populate();
        this.dialog.on();
    }
}

var pageSettingsInstance = controller.create<PageSettingsController, void, void>('pageSettings', PageSettingsController)[0];
var deletePageConfirmation = controller.create<DeletePageConfirmationController, void, void>('deletePageConfirm', DeletePageConfirmationController)[0];

class NavButtonController {
    open() {
        pageSettingsInstance.open();
    }
}

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.add("SettingsNavItem", NavButtonController);