"use strict";

jsns.run(function (using) {
    var form = using("htmlrest.form");
    var storage = using("htmlrest.storage");
    var component = using("htmlrest.components");
    var rest = using("htmlrest.rest");
    var BindingCollection = using("htmlrest.bindingcollection").BindingCollection;

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

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "CommitNavItem",
        created: function (button) {
            button.setListener({
                commit: function () {
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