"use strict";

jsns.run([
    "htmlrest.controller",
    "htmlrest.formlifecycle",
    "htmlrest.storage",
    "htmlrest.widgets.navmenu"
],
function (exports, module, controller, FormLifecycle, storage, navmenu) {

    function PageSettingsController(bindings) {
        var formLifecycle = new FormLifecycle(bindings);
        var dialog = bindings.getToggle('dialog');

        function NavButtonController(button) {
            function open() {
                formLifecycle.populate();
                dialog.on();
            }
            this.open = open;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("SettingsNavItem", NavButtonController);
    }

    controller.create('pageSettings', PageSettingsController);
});