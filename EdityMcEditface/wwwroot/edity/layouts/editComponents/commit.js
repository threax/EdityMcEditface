"use strict";

jsns.run(function (using) {
    var form = using("htmlrest.form");
    var storage = using("htmlrest.storage");
    var component = using("htmlrest.components");
    var rest = using("htmlrest.rest");

    $(".git-commit-form").submit(function (event) {
        var data = form.serialize($(this));
        rest.post('edity/Git/Commit', data,
        function (data) {
            $('#commitModal').modal('hide');
        }, function (data) {
            alert('Error Committing');
        });
        return false;
    });

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "CommitNavItem",
        created: function (button) {
            button.bind({
                CommitButton: {
                    click: function () {
                        rest.get('edity/Git/UncommittedChanges', function (data) {
                            var parent = $('.git-uncommitted-changes-list');
                            component.empty(parent);
                            component.repeat("git-uncommitted-change", parent, data);
                        });
                    }
                }
            });
        }});
});