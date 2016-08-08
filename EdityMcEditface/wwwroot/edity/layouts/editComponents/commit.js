"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu"
],
function (exports, module, storage, rest, controller, navmenu) {
    function determineCommitVariant(data) {
        return data.state;
    }

    function CommitController(commitDialog) {
        var commitModel = commitDialog.getModel('commit');
        var dialog = commitDialog.getToggle('dialog');

        function commit(evt) {
            evt.preventDefault();
            var data = commitModel.getData();
            rest.post(commitModel.getSrc(), data,
                function (resultData) {
                    commitModel.clear();
                    dialog.off();
                },
                function (resultData) {
                    alert('Error Committing');
                });
        }
        this.commit = commit;

        function NavButtonController(created) {
            function commit() {
                dialog.on();
                var changedFiles = commitDialog.getModel('changedFiles');
                rest.get(changedFiles.getSrc(), function (data) {
                    changedFiles.setData(data, null, determineCommitVariant);
                },
                function (data) {
                    alert('Cannot get uncommitted changes. Please try again later.');
                });
            }
            this.commit = commit;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("CommitNavItem", NavButtonController);
    }

    controller.create("commit", CommitController);
});