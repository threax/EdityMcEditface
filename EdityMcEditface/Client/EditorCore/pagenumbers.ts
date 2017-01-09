import * as toggles from 'hr.toggles';
import { ActionEventDispatcher } from 'hr.eventdispatcher';
import * as controller from 'hr.controller';

class PageNumberToggle extends toggles.OnOffToggle{
    private static pageStates = ["on", "off", "active"];

    public active() {
        this.applyState("active");
    }

    public getPossibleStates() {
        return PageNumberToggle.pageStates;
    }
}

export function PageNumbers(model, toggleProvider: controller.BindingCollection) {
    var pageToggles = [];
    var totalPages = 0;
    var buttonGroup = new toggles.Group();
    findToggles();
    var numButtons = pageToggles.length;
    var halfButton = Math.floor(numButtons / 2);
    var pageChangeRequested = new ActionEventDispatcher<number>();
    this.pageChangeRequested = pageChangeRequested.modifier;
    var lowestDisplayedPage = 0;
    var self = this;

    this.currentPage = 0;
    this.totalResults = 0;
    this.resultsPerPage = 0;

    function moveToPage(newPageNum) {
        pageChangeRequested.fire(newPageNum);
    }

    function pageNumberLink(index) {
        return function () {
            moveToPage(lowestDisplayedPage + index);
        }
    }

    function next() {
        var page = self.currentPage + 1;
        if (page < totalPages) {
            moveToPage(page)
        }
    }

    function previous() {
        var page = self.currentPage - 1;
        if (page >= 0) {
            moveToPage(page)
        }
    }

    function findToggles() {
        var bindings = {
            previousPage: function (evt) {
                evt.preventDefault();
                previous();
            },
            nextPage: function (evt) {
                evt.preventDefault();
                next();
            }
        };
        var t = 0;
        var currentPage = 'page' + t;
        var toggle = toggleProvider.getCustomToggle(currentPage, new PageNumberToggle());
        while (toggle.isUsable()) {
            pageToggles.push(toggle);
            buttonGroup.add(toggle);
            bindings[currentPage] = pageNumberLink(t);
            currentPage = 'page' + ++t;
            toggle = toggleProvider.getCustomToggle(currentPage, new PageNumberToggle());
        }
        toggleProvider.setListener(bindings);
    }

    function updatePages() {
        totalPages = Math.floor(this.totalResults / this.resultsPerPage);
        if (this.totalResults % this.resultsPerPage !== 0) {
            ++totalPages;
        }

        var j = 0;
        var i;

        if (this.currentPage + halfButton > totalPages) {
            i = totalPages - numButtons;
        }
        else {
            i = this.currentPage - halfButton;
        }
        if (i < 0) {
            i = 0;
        }
        lowestDisplayedPage = i;
        model.setData(function (page) {
            if (i === self.currentPage) {
                buttonGroup.activate(pageToggles[j], 'active', 'on');
            }
            if (i >= totalPages) {
                pageToggles[j].off();
            }
            ++j;
            return i++ + 1;
        });
    }
    this.updatePages = updatePages;
}