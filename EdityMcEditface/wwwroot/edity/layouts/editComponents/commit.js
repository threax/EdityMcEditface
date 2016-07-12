(function ($, h) {
    //$('#CommitButton').click();

    //--------------- File Browser ----------------

    $(".git-commit-form").submit(function (event) {
        var data = h.form.serialize($(this));
        h.rest.post('edity/Git/Commit', data,
        function (data) {
            $('#commitModal').modal('hide');
        }, function (data) {
            alert('Error Committing');
        });
        return false;
    });

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "CommitNavItem",
        created: function(button){
            button.addEventListener('click', function () {
                h.rest.get('edity/Git/UncommittedChanges', function (data) {
                    var parent = $('.git-uncommitted-changes-list');
                    h.component.empty(parent);
                    h.component.repeat("git-uncommitted-change", parent, data);
                });
            });
        }});
})(jQuery, htmlrest);