"use strict";


import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as navmenu from "edity.editorcore.navmenu";
import * as toggles from "hr.toggles";
import * as Iterable from "hr.iterable";
import * as GitService from "edity.editorcore.GitService";

class SyncController {
    private commitModel;
    private dialog;

    private load;
    private main;
    private cantSync;
    private error;
    private noChanges;
    private group;

    private changesModel;
    private behindHistory;
    private aheadHistory;

    constructor(bindings: controller.BindingCollection) {
        this.commitModel = bindings.getModel('commit');
        this.dialog = bindings.getToggle('dialog');

        this.load = bindings.getToggle('load');
        this.main = bindings.getToggle('main');
        this.cantSync = bindings.getToggle('cantSync');
        this.error = bindings.getToggle('error');
        this.noChanges = bindings.getToggle('noChanges');
        this.group = new toggles.Group(this.load, this.main, this.cantSync, this.error, this.noChanges);

        this.changesModel = bindings.getModel('changes');
        this.behindHistory = bindings.getModel('behindHistory');
        this.aheadHistory = bindings.getModel('aheadHistory');
    }

    push(evt) {
        evt.preventDefault();
        this.group.activate(this.load);
        GitService.push()
            .then((data) => GitService.syncInfo())
            .then((data) => this.setupSyncInfo(data))
            .catch((data) => this.group.activate(this.error));
    }

    pull(evt) {
        evt.preventDefault();
        this.group.activate(this.load);
        GitService.pull()
            .then((data) => GitService.syncInfo())
            .then((data) => this.setupSyncInfo(data))
            .catch((data) => this.group.activate(this.error));
    }

    private setupSyncInfo(data) {
        if (data.hasUncomittedChanges) {
            this.group.activate(this.cantSync);
        }
        else {
            if (data.aheadBy === 0 && data.behindBy === 0) {
                this.group.activate(this.noChanges);
            }
            else {
                this.group.activate(this.main);
                this.changesModel.setData(data);
                this.behindHistory.setData(new Iterable.Iterable(data.behindHistory).select(SyncController.formatRow));
                this.aheadHistory.setData(new Iterable.Iterable(data.aheadHistory).select(SyncController.formatRow));
            }
        }
    }

    startSync() {
        this.group.activate(this.load);
        this.dialog.on();

        GitService.syncInfo()
            .then((data) => this.setupSyncInfo(data))
            .catch((err) => this.group.activate(this.error));
    }

    private static formatRow(row) {
        var date = new Date(row.when);
        row.when = date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
        return row;
    }
}

var syncInstance = controller.create<SyncController, void, void>("sync", SyncController)[0];

class NavButtonController {
    sync() {
        syncInstance.startSync();
    }
}

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.add("SyncNavItem", NavButtonController);