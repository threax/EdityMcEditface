"use strict";

jsns.run(function (using) {
    var BindingCollection = using("htmlrest.bindingcollection").BindingCollection;
    var FormLifecycle = using("htmlrest.formlifecycle").FormLifecycle;
    var storage = using("htmlrest.storage");
    var toggles = using("htmlrest.toggles");
    var rest = using("htmlrest.rest");

    var bindings = new BindingCollection('#settingsModal');
    var formLifecycle = new FormLifecycle(bindings);

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SettingsNavItem",
        created: function (button) {
            button.setListener({
                open: function () {
                    formLifecycle.populate();
                }
            });
        }
    });
});