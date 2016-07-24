"use strict";

jsns.run([
    "htmlrest.bindingcollection",
    "htmlrest.formlifecycle",
    "htmlrest.storage",
    "htmlrest.toggles",
    "htmlrest.rest"
],
function(exports, module, BindingCollection, FormLifecycle, storage, toggles, rest) {

    var bindings = new BindingCollection('#settingsModal');
    var formLifecycle = new FormLifecycle(bindings);
    var dialog = bindings.getToggle('dialog');

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SettingsNavItem",
        created: function (button) {
            button.setListener({
                open: function () {
                    formLifecycle.populate();
                    dialog.on();
                }
            });
        }
    });
});