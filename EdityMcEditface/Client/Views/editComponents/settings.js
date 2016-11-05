"use strict";

jsns.run([
    "hr.controller",
    "hr.formlifecycle",
    "hr.storage",
    "hr.http",
    "hr.widgets.navmenu"
],
function (exports, module, controller, FormLifecycle, storage, http, navmenu) {

    var deletePageConfirmation;

    function DeletePageConfirmationController(bindings) {
        deletePageConfirmation = this;

        var dialog = bindings.getToggle('dialog');
        var model = bindings.getModel("info");
        var uploadUrl = model.getSrc();
        var currentUrl;
        
        function deletePage(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            http.delete(uploadUrl + currentUrl);
            currentUrl = null;
            dialog.off();
        }
        this.deletePage = deletePage;

        function confirmDelete(name, url) {
            model.setData(name);
            currentUrl = url;
            dialog.on();
        }
        this.confirmDelete = confirmDelete;
    }

    function PageSettingsController(bindings) {
        var formLifecycle = new FormLifecycle.FormLifecycle(bindings);
        var dialog = bindings.getToggle('dialog');

        function deletePage(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            deletePageConfirmation.confirmDelete(document.title, window.location.pathname);
            dialog.off();
        }
        this.deletePage = deletePage;

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
    controller.create('deletePageConfirm', DeletePageConfirmationController);
});