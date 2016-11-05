"use strict";

import {TimedTrigger} from 'hr.timedtrigger';
import {EventHandler} from 'hr.eventhandler';
import { PromiseEventHandler } from 'hr.promiseeventhandler';

var allowSave = true;
var saveAgainWhenSaveCompleted = false;
var outstandingSaveRequest = false;

var saveTrigger = new TimedTrigger(5000);
var saveStartedEventHandler = new EventHandler();
var saveCompletedEventHandler = new EventHandler();
var saveErrorEventHandler = new EventHandler();
var saveEventHandler = new PromiseEventHandler();
export const saveStartedEvent = saveStartedEventHandler.modifier;
export const saveCompletedEvent = saveCompletedEventHandler.modifier;
export const saveErrorEvent = saveErrorEventHandler.modifier;
export const saveEvent = saveEventHandler.modifier;

function doSave() {
    outstandingSaveRequest = false;
    allowSave = false;
    saveStartedEventHandler.fire();
    saveEventHandler.fire()
        .then(function (data) {
            saveCompletedEventHandler.fire();
            finishSave();
        })
        .catch(function (data) {
            saveErrorEventHandler.fire();
            finishSave();
        });
}
saveTrigger.addListener(this, doSave);

export function requestSave() {
    if (allowSave) {
        outstandingSaveRequest = true;
        saveTrigger.fire();
    }
    else {
        saveAgainWhenSaveCompleted = true;
    }
}

export function saveNow() {
    saveTrigger.cancel();
    if (allowSave) {
        doSave();
    }
    else {
        saveAgainWhenSaveCompleted = true;
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