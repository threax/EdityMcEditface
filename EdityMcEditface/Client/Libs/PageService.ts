"use strict";

import * as http from 'hr.http';
import * as saveService from 'clientlibs.SaveService';

var sourceAccessor;
var needsSave = false;

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
        return http.upload('/edity/Page/' + window.location.pathname, blob)
            .catch(function (err) {
                needsSave = true;
                throw err;
            });
    }
}
saveService.saveEvent.add(this, doSave);