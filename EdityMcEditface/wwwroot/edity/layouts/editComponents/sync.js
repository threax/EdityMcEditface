"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "htmlrest.toggles",
    "htmlrest.iter",
    "edity.GitService"
],
function (exports, module, storage, controller, navmenu, toggles, iter, GitService) {
    function SyncController(bindings) {
        var commitModel = bindings.getModel('commit');
        var dialog = bindings.getToggle('dialog');

        var load = bindings.getToggle('load');
        var main = bindings.getToggle('main');
        var cantSync = bindings.getToggle('cantSync');
        var error = bindings.getToggle('error');
        var group = new toggles.Group(load, main, cantSync, error);

        var changesModel = bindings.getModel('changes');
        var behindHistory = bindings.getModel('behindHistory');
        var aheadHistory = bindings.getModel('aheadHistory');

        function push(evt) {
            evt.preventDefault();
            group.activate(load);
            GitService.push()
            .then(function (data) {
                return GitService.syncInfo();
            })
            .then(setupSyncInfo)
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
                return GitService.syncInfo();
            })
            .then(setupSyncInfo)
            .catch(function (data) {
                group.activate(error);
            });
        }
        this.pull = pull;

        function setupSyncInfo(data) {
            if (data.hasUncomittedChanges) {
                group.activate(cantSync);
            }
            else {
                group.activate(main);
                changesModel.setData(data);
                behindHistory.setData(iter(data.behindHistory, formatRow));
                aheadHistory.setData(iter(data.aheadHistory, formatRow));
            }
        }

        function NavButtonController(created) {
            function sync() {
                group.activate(load);
                dialog.on();

                GitService.syncInfo()
                .then(setupSyncInfo)
                .catch(function (data) {
                    group.activate(error);
                });
            }
            this.sync = sync;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("SyncNavItem", NavButtonController);
    }

    function formatRow(row) {
        var date = new Date(row.when);
        row.when = date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
        return row;
    }

    controller.create("sync", SyncController);
});