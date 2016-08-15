"use strict";

jsns.run([
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "htmlrest.toggles",
    "edity.GitService"
],
function (exports, module, controller, navmenu, toggles, GitService) {
    function HistoryController(bindings) {
        var dialog = bindings.getToggle('dialog');

        var main = bindings.getToggle('main');
        var load = bindings.getToggle('load');
        var error = bindings.getToggle('error');
        var toggleGroup = new toggles.Group(main, load, error);

        var historyModel = bindings.getModel('history');

        function NavButtonController(created) {
            function history() {
                dialog.on();
                toggleGroup.activate(load);
                GitService.history(window.location.pathname)
                .then(function (data) {
                    historyModel.setData(data);
                    toggleGroup.activate(main);
                })
                .catch(function (data) {
                    toggleGroup.activate(error);
                });
            }
            this.history = history;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("HistoryNavItem", NavButtonController);
    }

    controller.create("history", HistoryController);
});