"use strict";

jsns.define("edity.commitSync", [
    "htmlrest.eventhandler"
],
function (exports, module, EventHandler) {
    var determineCommitVariantEvent = new EventHandler();

    function fireDetermineCommitVariant(data) {
        return determineCommitVariantEvent.fire(data);
    }

    exports.determineCommitVariantEvent = determineCommitVariantEvent.modifier;
    exports.fireDetermineCommitVariant = fireDetermineCommitVariant;
})

jsns.run([
    "htmlrest.storage",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "htmlrest.toggles",
    "edity.commitSync",
    "edity.GitService"
],
function (exports, module, storage, controller, navmenu, toggles, commitSync, GitService) {
    var currentRowCreatedCallback;

    function determineCommitVariant(data) {
        var listenerVariant = commitSync.fireDetermineCommitVariant(data);
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
        var toggleGroup = new toggles.Group(main, load, error);

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
                var changedFiles = commitDialog.getModel('changedFiles');
                GitService.uncommittedChanges()
                    .then(function (data) {
                        toggleGroup.activate(main);
                        changedFiles.setData(data, commitRowCreated, determineCommitVariant);
                    })
                    .catch(function (data) {
                        toggleGroup.activate(error);
                    });
            }
            this.commit = commit;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("CommitNavItem", NavButtonController);
    }

    controller.create("commit", CommitController);
});