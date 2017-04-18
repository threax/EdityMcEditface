///<amd-module name="edity.core.edit.components.settings"/>

"use strict";

import * as controller from "hr.controller";
import * as FormLifecycle from "edity.editorcore.FormLifecycle";
import * as storage from "hr.storage";
import { PageClient, PageSettings } from "edity.editorcore.EdityClient";
import * as navmenu from "edity.editorcore.navmenu";
import * as Toggles from 'hr.toggles';
import * as PageStart from 'edity.editorcore.EditorPageStart';

class DeletePageConfirmationController {
    private dialog;
    private model;
    private uploadUrl;
    private currentUrl;
    private client: PageClient;

    constructor(bindings, client: PageClient) {
        this.dialog = bindings.getToggle('dialog');
        this.model = bindings.getModel("info");
        this.uploadUrl = this.model.getSrc();
        this.currentUrl;
        this.client = client;
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

interface PageSettingsContext {
    client: PageClient,
    deletePageConfirmation: DeletePageConfirmationController
}

class PageSettingsController {
    private dialog;
    private client: PageClient;
    private main: Toggles.Toggle;
    private error: Toggles.Toggle;
    private load: Toggles.Toggle;
    private toggles: Toggles.Group;
    private settings: controller.Model<PageSettings>;
    private deletePageConfirmation: DeletePageConfirmationController;

    constructor(bindings: controller.BindingCollection, context: PageSettingsContext) {
        this.dialog = bindings.getToggle('dialog');
        this.client = context.client;

        this.main = bindings.getToggle('main');
        this.error = bindings.getToggle('error');
        this.load = bindings.getToggle('load');
        this.toggles = new Toggles.Group(this.main, this.error, this.load);

        this.settings = bindings.getModel<PageSettings>("settings");
        this.deletePageConfirmation = context.deletePageConfirmation;
    }

    deletePage(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        this.deletePageConfirmation.confirmDelete(document.title, window.location.pathname);
        this.dialog.off();
    }

    open() {
        this.toggles.activate(this.load);
        this.client.getSettings(window.location.pathname, null)
            .then(data => {
                this.settings.setData(data);
                this.toggles.activate(this.main);
            })
            .catch(err => {
                this.toggles.activate(this.error);
            });
        this.dialog.on();
    }

    submit(evt) {
        evt.preventDefault();
        this.toggles.activate(this.load);
        var data = this.settings.getData();
        this.client.updateSettings(window.location.pathname, data, null)
            .then(result => {
                this.toggles.activate(this.main);
                this.dialog.off();
            })
            .catch(err => {
                this.toggles.activate(this.error);
            });
    }
}

PageStart.init().then(config => {
    var client = new PageClient(config.BaseUrl, config.Fetcher);
    var deletePageConfirmation = controller.create<DeletePageConfirmationController, PageClient, void>('deletePageConfirm', DeletePageConfirmationController, client)[0];
    var pageSettingsInstance = controller.create<PageSettingsController, PageSettingsContext, void>('pageSettings', PageSettingsController,
        {
            client: client,
            deletePageConfirmation: deletePageConfirmation
        })[0];

    class NavButtonController {
        open(evt) {
            evt.preventDefault();
            pageSettingsInstance.open();
        }
    }

    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("SettingsNavItem", NavButtonController);
});