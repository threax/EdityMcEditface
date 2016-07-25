"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller"
],
function (exports, module, storage, rest, controller) {
    function CommitController(commitDialog) {
        var commitModel = commitDialog.getModel('commit');

        function commit(evt) {
            evt.preventDefault();
            var data = commitModel.getData();
            rest.post(commitModel.getSrc(), data,
                function (resultData) {
                    $('#commitModal').modal('hide'); //Still jquery because of bootstrap
                },
                function (resultData) {
                    alert('Error Committing');
                });
        }
        this.commit = commit;

        var dialog = commitDialog.getToggle('dialog');

        var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
        buttonCreation.push({
            name: "CommitNavItem",
            created: function (button) {
                button.setListener({
                    commit: function () {
                        dialog.on();
                        var changedFiles = commitDialog.getModel('changedFiles');
                        rest.get(changedFiles.getSrc(), function (data) {
                            changedFiles.setData(data);
                        },
                        function (data) {
                            alert('Cannot get uncommitted changes. Please try again later.');
                        });
                    }
                });
            }
        });
    }

    controller.create("commit", CommitController);
});