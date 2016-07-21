"use strict";

jsns.run(function (using) {
    var BindingCollection = using("htmlrest.bindingcollection");
    var FormLifecycle = using("htmlrest.formlifecycle");
    var storage = using("htmlrest.storage");

    var settingsBindings = new BindingCollection('#settingsModal');
    var settingsLifecycle = new FormLifecycle(settingsBindings);

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SettingsNavItem",
        created: function (button) {
            button.setListener({
                open: function(){
                    settingsLifecycle.populateData();
                }
            });
        }
    });
});