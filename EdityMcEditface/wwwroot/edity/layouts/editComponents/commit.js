"use strict";

jsns.run([
    "htmlrest.form",
    "htmlrest.storage",
    "htmlrest.components",
    "htmlrest.rest",
    "htmlrest.bindingcollection"
],
function (exports, module, form, storage, component, rest, BindingCollection) {
    var commitDialog = new BindingCollection('#commitModal');
    var commitModel = commitDialog.getModel('commit');
    commitDialog.setListener({
        commit: function (evt) {
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
    });

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
        }});
});