"use strict";

import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as navmenu from "edity.editorcore.navmenu";
import * as toggles from "hr.toggles";
import * as GitService from "edity.editorcore.GitService";
import * as saveService from "edity.editorcore.SaveService";

var currentRowCreatedCallback;

function determineCommitVariant(data) {
    var listenerVariant = GitService.fireDetermineCommitVariant(data);
    if (listenerVariant) {
        currentRowCreatedCallback = listenerVariant[0].rowCreated;
        return listenerVariant[0].variant;
    }
    return data.state;
}

function commitRowCreated(bindings, data) {
    if (currentRowCreatedCallback) {
        currentRowCreatedCallback(bindings, data);
        currentRowCreatedCallback = null;
    }
}

class NavButtonController {
    constructor(bindings) {

    }

    commit() {
        saveService.saveNow()
        .then(r => {
            commitController.startCommit();
        })
    }
}

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.add("CommitNavItem", NavButtonController);

class CommitController {
    private commitModel;
    private dialog;

    private main;
    private load;
    private error;
    private noChanges;
    private toggleGroup;
    private changedFiles;

    constructor(commitDialog) {
        this.commitModel = commitDialog.getModel('commit');
        this.dialog = commitDialog.getToggle('dialog');

        this.main = commitDialog.getToggle('main');
        this.load = commitDialog.getToggle('load');
        this.error = commitDialog.getToggle('error');
        this.noChanges = commitDialog.getToggle('noChanges');
        this.toggleGroup = new toggles.Group(this.main, this.load, this.error, this.noChanges);
        this.changedFiles = commitDialog.getModel('changedFiles');

        GitService.revertStarted.add(() => this.toggleGroup.activate(this.load));

        GitService.revertCompleted.add((success) => {
            if (success) {
                this.updateUncommittedFiles();
            }
            else {
                this.toggleGroup.activate(this.error);
            }
        });
    }

    private updateUncommittedFiles() {
        GitService.uncommittedChanges()
            .then((data: any[]) => {
                if (data.length > 0) {
                    this.toggleGroup.activate(this.main);
                    this.changedFiles.setData(data, commitRowCreated, determineCommitVariant);
                }
                else {
                    this.toggleGroup.activate(this.noChanges);
                }
            })
            .catch((data) => {
                this.toggleGroup.activate(this.error);
            });
    }

    commit(evt) {
        evt.preventDefault();
        this.toggleGroup.activate(this.load);
        var data = this.commitModel.getData();
        GitService.commit(data)
            .then((resultData) => {
                this.toggleGroup.activate(this.main);
                this.commitModel.clear();
                this.dialog.off();
            })
            .catch((errorData) => {
                this.toggleGroup.activate(this.main);
                alert('Error Committing');
            });
    }

    startCommit() {
        this.toggleGroup.activate(this.load);
        this.dialog.on();
        this.updateUncommittedFiles();
    }
}

var commitController = controller.create<CommitController, void, void>("commit", CommitController)[0];