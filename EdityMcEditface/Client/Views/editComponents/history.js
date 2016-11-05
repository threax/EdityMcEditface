"use strict";

jsns.run([
    "hr.controller",
    "hr.widgets.navmenu",
    "hr.toggles",
    "hr.widgets.pagenumbers",
    "hr.iterable",
    "edity.GitService"
],
function (exports, module, controller, navmenu, toggles, PageNumbers, Iterable, GitService) {
    function HistoryController(bindings) {
        var dialog = bindings.getToggle('dialog');

        var main = bindings.getToggle('main');
        var load = bindings.getToggle('load');
        var error = bindings.getToggle('error');
        var toggleGroup = new toggles.Group(main, load, error);

        var historyModel = bindings.getModel('history');

        var pagedData = GitService.createHistoryPager(window.location.pathname, 10);
        pagedData.updating.add(this, dataUpdating);
        pagedData.updated.add(this, dataUpdated);
        pagedData.error.add(this, loadFailed);

        function dataUpdating() {
            toggleGroup.show(load);
        }

        function dataUpdated(data) {
            historyModel.setData(new Iterable(data).select(function(item){
                item.when = new Date(item.when).toLocaleString();
                return item;
            }));
            toggleGroup.activate(main);
            pageNumbers.currentPage = pagedData.currentPage;
            pageNumbers.updatePages();
            toggleGroup.show(main);
        }

        function loadFailed() {
            toggleGroup.show(error);
        }

        var pageNumbers = new PageNumbers(bindings.getModel('pageNumbers'), bindings);
        pageNumbers.resultsPerPage = pagedData.resultsPerPage;
        pageNumbers.pageChangeRequested.add(this, pageChangeRequested);

        function pageChangeRequested(pageNum) {
            pagedData.currentPage = pageNum;
            pagedData.updateData();
        }

        function NavButtonController(created) {
            function history() {
                dialog.on();
                toggleGroup.activate(load);
                pagedData.updateData();
                GitService.historyCount(window.location.pathname)
                .then(function (data) {
                    pageNumbers.totalResults = data;
                    pageNumbers.currentPage = 0;
                    pageNumbers.updatePages();
                });
            }
            this.history = history;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("HistoryNavItem", NavButtonController);
    }

    controller.create("history", HistoryController);
});