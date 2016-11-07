"use strict";

import * as http from 'hr.http';
import * as uploader from 'clientlibs.uploader';
import { EventHandler } from 'hr.eventhandler';
import { PagedData } from 'hr.widgets.pageddata';

var host = "";

export function setHost(url) {
    host = url;
}

export function syncInfo() {
    return http.get(host + '/edity/Git/SyncInfo');
}

export function uncommittedChanges() {
    return http.get(host + '/edity/Git/UncommittedChanges');
}

export function commit(data) {
    return http.post(host + '/edity/Git/Commit', data);
}

export function uncommittedDiff(file) {
    return http.get(host + '/edity/Git/UncommittedDiff/' + file);
}

export function mergeInfo(file) {
    return http.get(host + '/edity/Git/MergeInfo/' + file);
}

export function historyCount(file) {
    return http.get(host + '/edity/Git/HistoryCount/' + file);
}

export function createHistoryPager(file, count) {
    return new PagedData(host + '/edity/Git/History/' + file, count);
}

export function resolve(file, content) {
    var blob = new Blob([content], { type: "text/html" });
    return uploader.upload(host + '/edity/Git/Resolve/' + file, blob);
}

export function pull() {
    return http.post(host + '/edity/Git/Pull');
}

export function push() {
    return http.post(host + '/edity/Git/Push');
}

var revertStartedHandler = new EventHandler();
var revertCompletedHandler = new EventHandler();

export function revert(file) {
    revertStartedHandler.fire();
    return http.post(host + '/edity/Git/Revert/' + file)
        .then(function (data) {
            revertCompletedHandler.fire(true);
        })
        .catch(function (data) {
            revertCompletedHandler.fire(false);
        });
}

export const revertStarted = revertStartedHandler.modifier;
export const revertCompleted = revertCompletedHandler.modifier;

//Commit variant detection and sync
var determineCommitVariantEventHandler = new EventHandler();

export function fireDetermineCommitVariant(data) {
    return determineCommitVariantEventHandler.fire(data);
}

export const determineCommitVariantEvent = determineCommitVariantEventHandler.modifier;