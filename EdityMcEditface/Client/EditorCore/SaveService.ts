"use strict";

import { TimedTrigger } from 'hr.timedtrigger';
import { ActionEventDispatcher, PromiseEventDispatcher } from 'hr.eventdispatcher';

var allowSave = true;
var saveAgainWhenSaveCompleted = false;
var outstandingSaveRequest = false;

var saveTrigger = new TimedTrigger(5000);
var saveStartedEventHandler = new ActionEventDispatcher<void>();
var saveCompletedEventHandler = new ActionEventDispatcher<void>();
var saveErrorEventHandler = new ActionEventDispatcher<void>();
var saveEventHandler = new PromiseEventDispatcher<void, void>();
export const saveStartedEvent = saveStartedEventHandler.modifier;
export const saveCompletedEvent = saveCompletedEventHandler.modifier;
export const saveErrorEvent = saveErrorEventHandler.modifier;
export const saveEvent = saveEventHandler.modifier;

function doSave() {
    outstandingSaveRequest = false;
    allowSave = false;
    saveStartedEventHandler.fire(null);
    return saveEventHandler.fire(null)
        .then(function (data) {
            saveCompletedEventHandler.fire(null);
            finishSave();
        })
        .catch(function (data) {
            saveErrorEventHandler.fire(null);
            finishSave();
        });
}
saveTrigger.addListener(doSave);

export function requestSave() {
    if (allowSave) {
        outstandingSaveRequest = true;
        saveTrigger.fire(null);
    }
    else {
        saveAgainWhenSaveCompleted = true;
    }
}

export function saveNow() : Promise<void> {
    saveTrigger.cancel();
    if (allowSave) {
        return doSave();
    }
    else {
        saveAgainWhenSaveCompleted = true;
        return Promise.resolve<void>();
    }
}

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