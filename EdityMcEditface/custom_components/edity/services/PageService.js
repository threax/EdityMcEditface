﻿"use strict";

jsns.define("edity.PageService", [
    "edity.SaveService",
    "htmlrest.rest"
], function (exports, module, saveService, rest) {
    var sourceAccessor;
    var needsSave = false;

    function setHtml(value) {
        sourceAccessor.setHtml(value);
        sourceUpdated();
    }
    exports.setHtml = setHtml;

    function getHtml() {
        return sourceAccessor.getHtml();
    }
    exports.getHtml = getHtml;

    function setSourceAccessor(value) {
        sourceAccessor = value;
    }
    exports.setSourceAccessor = setSourceAccessor;

    function sourceUpdated() {
        saveService.requestSave();
        needsSave = true;
    }
    exports.sourceUpdated = sourceUpdated;

    function doSave() {
        if (needsSave) {
            needsSave = false;
            var content = exports.getHtml();
            var blob = new Blob([content], { type: "text/html" });
            return rest.uploadPromise('/edity/Page/' + window.location.pathname, blob)
            .catch(function (data) {
                needsSave = true;
            });
        }
    }
    saveService.saveEvent.add(this, doSave);
});