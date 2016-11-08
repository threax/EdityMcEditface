"use strict";

import * as edityClient from 'clientlibs.EdityClient';
import * as saveService from 'clientlibs.SaveService';
import * as pageStart from 'clientlibs.PageStart';

var sourceAccessor;
var needsSave = false;
var client: edityClient.PageClient;

export function setHtml(value) {
    sourceAccessor.setHtml(value);
    sourceUpdated();
}

export function getHtml() {
    return sourceAccessor.getHtml();
}

export function setSourceAccessor(value) {
    sourceAccessor = value;
}

export function sourceUpdated() {
    saveService.requestSave();
    needsSave = true;
}

function doSave() {
    if (needsSave) {
        needsSave = false;
        var content = getHtml();
        var blob = new Blob([content], { type: "text/html" });
        return client.save(window.location.pathname, { fileName: window.location.pathname, data: blob }, null)
            .catch(function (err) {
                needsSave = true;
                throw err;
            });
    }
}

pageStart.init().then((config) => {
    client = new edityClient.PageClient(config.BaseUrl, config.Fetcher);
    saveService.saveEvent.add(this, doSave);
});