"use strict";

import * as http from 'hr.http';
import { ActionEventDispatcher } from 'hr.eventdispatcher';

export class PagedData<T> {
    private updatingEvent = new ActionEventDispatcher<PagedData<T>>();
    private updatedEvent = new ActionEventDispatcher<T>();
    private errorEvent = new ActionEventDispatcher<T>();
    private currentPage: number = 0;
    private resultsPerPage: number;
    private src: string;

    constructor(src, resultsPerPage) {
        this.src = src;
        this.resultsPerPage = resultsPerPage;
    }

    public updateData() {
        this.updatingEvent.fire(this);
        var url = this.src + '?page=' + this.currentPage + '&count=' + this.resultsPerPage;
        http.get(url)
            .then(function (data) {
                this.updated.fire(data);
            })
            .catch(function (data) {
                this.error.fire(data);
            });
    }

    get updating1() {
        return this.updatingEvent.modifier;
    }

    get updated1() {
        return this.updatedEvent.modifier;
    }

    get error1() {
        return this.errorEvent.modifier;
    }
}

/**
 * This class calls a callback function to get data.
 */
export class PagedClientData<T> {
    private listFunc;
    private updatingEvent = new ActionEventDispatcher<PagedClientData<T>>();
    private updatedEvent = new ActionEventDispatcher<T>();
    private errorEvent = new ActionEventDispatcher<T>();
    resultsPerPage;
    currentPage = 0;

    constructor(listFunc: (page: number, resultsPerPage: number) => Promise<T>, resultsPerPage) {
        this.listFunc = listFunc;
        this.resultsPerPage = resultsPerPage;
        this.resultsPerPage = resultsPerPage;
    }

    updateData() {
        this.updatingEvent.fire(this);
        this.listFunc(this.currentPage, this.resultsPerPage)
            .then((data) => {
                this.updatedEvent.fire(data);
            })
            .catch((data) => {
                this.errorEvent.fire(data);
            });
    }

    get updating() {
        return this.updatingEvent.modifier;
    }

    get updated() {
        return this.updatedEvent.modifier;
    }

    get error() {
        return this.errorEvent.modifier;
    }
}