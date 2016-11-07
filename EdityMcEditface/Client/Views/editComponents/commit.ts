"use strict";

import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as navmenu from "hr.widgets.navmenu";
import * as toggles from "hr.toggles";
import * as GitService from "clientlibs.GitService";

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

function CommitController(commitDialog) {
    var commitModel = commitDialog.getModel('commit');
    var dialog = commitDialog.getToggle('dialog');

    var main = commitDialog.getToggle('main');
    var load = commitDialog.getToggle('load');
    var error = commitDialog.getToggle('error');
    var noChanges = commitDialog.getToggle('noChanges');
    var toggleGroup = new toggles.Group(main, load, error, noChanges);
    var changedFiles = commitDialog.getModel('changedFiles');

    function updateUncommittedFiles() {
        GitService.uncommittedChanges()
            .then(function (data:any[]) {
                if (data.length > 0) {
                    toggleGroup.activate(main);
                    changedFiles.setData(data, commitRowCreated, determineCommitVariant);
                }
                else {
                    toggleGroup.activate(noChanges);
                }
            })
            .catch(function (data) {
                toggleGroup.activate(error);
            });
    }

    GitService.revertStarted.add(this, function () {
        toggleGroup.activate(load);
    });

    GitService.revertCompleted.add(this, function (success) {
        if (success) {
            updateUncommittedFiles();
        }
        else {
            toggleGroup.activate(error);
        }
    });

    function commit(evt) {
        evt.preventDefault();
        toggleGroup.activate(load);
        var data = commitModel.getData();
        GitService.commit(data)
            .then(function (resultData) {
                toggleGroup.activate(main);
                commitModel.clear();
                dialog.off();
            })
            .catch(function (errorData) {
                toggleGroup.activate(main);
                alert('Error Committing');
            });
    }
    this.commit = commit;

    function NavButtonController(created) {
        function commit() {
            toggleGroup.activate(load);
            dialog.on();
            updateUncommittedFiles();
        }
        this.commit = commit;
    }

    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("CommitNavItem", NavButtonController);
}

controller.create("commit", CommitController);