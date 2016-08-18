"use strict";

jsns.define("edity.SaveService", [
    "hr.timedtrigger",
    "hr.eventhandler",
    "hr.promiseeventhandler",
], function (exports, module, TimedTrigger, EventHandler, PromiseEventHandler) {
    var allowSave = true;
    var saveAgainWhenSaveCompleted = false;
    var outstandingSaveRequest = false;

    var saveTrigger = new TimedTrigger(5000);
    var saveStartedEvent = new EventHandler();
    var saveCompletedEvent = new EventHandler();
    var saveErrorEvent = new EventHandler();
    var saveEvent = new PromiseEventHandler();
    exports.saveStartedEvent = saveStartedEvent.modifier;
    exports.saveCompletedEvent = saveCompletedEvent.modifier;
    exports.saveErrorEvent = saveErrorEvent.modifier;
    exports.saveEvent = saveEvent.modifier;

    function doSave() {
        outstandingSaveRequest = false;
        allowSave = false;
        saveStartedEvent.fire();
        saveEvent.fire()
        .then(function (data) {
            saveCompletedEvent.fire();
            finishSave();
        })
        .catch(function (data) {
            saveErrorEvent.fire();
            finishSave();
        });
    }
    saveTrigger.addListener(this, doSave);

    function requestSave() {
        if (allowSave) {
            outstandingSaveRequest = true;
            saveTrigger.fire();
        }
        else {
            saveAgainWhenSaveCompleted = true;
        }
    }
    exports.requestSave = requestSave;

    function saveNow() {
        saveTrigger.cancel();
        if (allowSave) {
            doSave();
        }
        else {
            saveAgainWhenSaveCompleted = true;
        }
    }
    exports.saveNow = saveNow;

    function finishSave() {
        allowSave = true;
        if (saveAgainWhenSaveCompleted) {
            saveAgainWhenSaveCompleted = false;
            doSave();
        }
    }

    window.addEventListener('beforeunload', function (evt) {
        if (outstandingSaveRequest) {
            doSave();
        }
    });
});