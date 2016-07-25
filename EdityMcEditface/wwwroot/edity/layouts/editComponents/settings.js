"use strict";

jsns.run([
    "htmlrest.controller",
    "htmlrest.formlifecycle",
    "htmlrest.storage"
],
function (exports, module, controller, FormLifecycle, storage) {

    function PageSettingsController(bindings) {
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
    }

    controller.create('pageSettings', PageSettingsController);
});