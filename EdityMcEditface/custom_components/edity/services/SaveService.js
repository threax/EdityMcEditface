"use strict";

jsns.define("edity.SaveService", [
    "htmlrest.timedtrigger",
    "htmlrest.eventhandler",
    "htmlrest.promiseeventhandler",
], function (exports, module, TimedTrigger, EventHandler, PromiseEventHandler) {
    var saveTrigger = new TimedTrigger(1000);
    var saveStartedEvent = new EventHandler();
    var saveCompletedEvent = new EventHandler();
    var saveErrorEvent = new EventHandler();
    var saveEvent = new PromiseEventHandler();
    exports.saveStartedEvent = saveStartedEvent.modifier;
    exports.saveCompletedEvent = saveCompletedEvent.modifier;
    exports.saveErrorEvent = saveErrorEvent.modifier;
    exports.saveEvent = saveEvent.modifier;

    function doSave() {
        saveStartedEvent.fire();
        saveEvent.fire()
        .then(function (data) {
            saveCompletedEvent.fire();
        })
        .catch(function (data) {
            saveErrorEvent.fire();
        });
    }
    saveTrigger.addListener(this, doSave);

    function requestSave() {
        saveTrigger.fire();
    }
    exports.requestSave = requestSave;
});