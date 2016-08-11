"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "htmlrest.toggles",
    "edity.GitService"
],
function (exports, module, storage, controller, navmenu, toggles, GitService) {
    function SyncController(bindings) {
        var commitModel = bindings.getModel('commit');
        var dialog = bindings.getToggle('dialog');

        var load = bindings.getToggle('load');
        var main = bindings.getToggle('main');
        var cantSync = bindings.getToggle('cantSync');
        var error = bindings.getToggle('error');
        var group = new toggles.Group(load, main, cantSync, error);

        function push(evt) {
            evt.preventDefault();
            group.activate(load);
            GitService.push()
            .then(function (data) {
                group.activate(main);
            })
            .catch(function(data){
                group.activate(error);
            });
        }
        this.push = push;

        function pull(evt) {
            evt.preventDefault();
            group.activate(load);
            GitService.pull()
            .then(function (data) {
                group.activate(main);
            })
            .catch(function (data) {
                group.activate(error);
            });
        }
        this.pull = pull;

        function NavButtonController(created) {
            function sync() {
                group.activate(load);
                dialog.on();

                GitService.hasUncommittedChanges()
                .then(function (data) {
                    if (data) {
                        group.activate(cantSync);
                    }
                    else {
                        group.activate(main);
                    }
                })
                .catch(function (data) {
                    group.activate(error);
                });
            }
            this.sync = sync;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("SyncNavItem", NavButtonController);
    }

    controller.create("sync", SyncController);
});