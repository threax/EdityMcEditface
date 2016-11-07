"use strict";

import * as controller from "hr.controller";
import * as navmenu from "hr.widgets.navmenu";
import * as toggles from "hr.toggles";
import * as PageNumbers from "hr.widgets.pagenumbers";
import * as Iterable from "hr.iterable";
import * as GitService from "clientlibs.GitService";

class NavButtonController {
    private controller: HistoryController;

    constructor(bindings: controller.BindingCollection, controller: HistoryController) {
        this.controller = controller;
    }

    history() {
        this.controller.showHistory();
    }
}

class HistoryController {
    private dialog;

    private main;
    private load;
    private error;
    private toggleGroup;

    private historyModel;

    private pagedData = GitService.createHistoryPager(window.location.pathname, 10);
    private pageNumbers;


    constructor(bindings: controller.BindingCollection) {
        this.dialog = bindings.getToggle('dialog');

        this.main = bindings.getToggle('main');
        this.load = bindings.getToggle('load');
        this.error = bindings.getToggle('error');
        this.toggleGroup = new toggles.Group(this.main, this.load, this.error);

        this.historyModel = bindings.getModel('history');

        this.pagedData = GitService.createHistoryPager(window.location.pathname, 10);
        this.pagedData.updating.add(this, this.dataUpdating);
        this.pagedData.updated.add(this, this.dataUpdated);
        this.pagedData.error.add(this, this.loadFailed);

        this.pageNumbers = new PageNumbers.PageNumbers(bindings.getModel('pageNumbers'), bindings);
        this.pageNumbers.resultsPerPage = this.pagedData.resultsPerPage;
        this.pageNumbers.pageChangeRequested.add(this, this.pageChangeRequested);

        //Add to nav menu
        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("HistoryNavItem", NavButtonController, this);
    }

    private dataUpdating() {
        this.toggleGroup.activate(this.load);
    }

    private dataUpdated(data) {
        this.historyModel.setData(new Iterable.Iterable(data).select(function(item){
            item.when = new Date(item.when).toLocaleString();
            return item;
        }));
        this.toggleGroup.activate(this.main);
        this.pageNumbers.currentPage = this.pagedData.currentPage;
        this.pageNumbers.updatePages();
        this.toggleGroup.activate(this.main);
    }

    private loadFailed() {
        this.toggleGroup.activate(this.error);
    }

    private pageChangeRequested(pageNum) {
        this.pagedData.currentPage = pageNum;
        this.pagedData.updateData();
    }

    showHistory() {
        this.dialog.on();
        this.toggleGroup.activate(this.load);
        this.pagedData.updateData();
        GitService.historyCount(window.location.pathname)
            .then((data) => {
                this.pageNumbers.totalResults = data;
                this.pageNumbers.currentPage = 0;
                this.pageNumbers.updatePages();
            });
    }
}

controller.create<HistoryController, void, void>("history", HistoryController, null);