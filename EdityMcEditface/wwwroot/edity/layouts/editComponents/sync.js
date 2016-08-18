"use strict";

jsns.run([
    "hr.storage",
    "hr.controller",
    "hr.widgets.navmenu",
    "hr.toggles",
    "hr.iterable",
    "edity.GitService"
],
function (exports, module, storage, controller, navmenu, toggles, Iterable, GitService) {
    function SyncController(bindings) {
        var commitModel = bindings.getModel('commit');
        var dialog = bindings.getToggle('dialog');

        var load = bindings.getToggle('load');
        var main = bindings.getToggle('main');
        var cantSync = bindings.getToggle('cantSync');
        var error = bindings.getToggle('error');
        var noChanges = bindings.getToggle('noChanges');
        var group = new toggles.Group(load, main, cantSync, error, noChanges);

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
                if (data.aheadBy === 0 && data.behindBy === 0) {
                    group.activate(noChanges);
                }
                else {
                    group.activate(main);
                    changesModel.setData(data);
                    behindHistory.setData(new Iterable(data.behindHistory).select(formatRow));
                    aheadHistory.setData(new Iterable(data.aheadHistory).select(formatRow));
                }
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