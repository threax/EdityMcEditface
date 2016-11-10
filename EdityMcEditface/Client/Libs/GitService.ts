"use strict";

import * as edityClient from 'clientlibs.EdityClient';
import * as uploader from 'clientlibs.uploader';
import { EventHandler } from 'hr.eventhandler';
import { PagedData } from 'hr.widgets.pageddata';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import * as pageStart from 'clientlibs.PageStart';

var host = "";
var cacheBuster = new CacheBuster(new WindowFetch());
var client: edityClient.GitClient;

pageStart.init()
    .then((config) => {
        client = new edityClient.GitClient(config.BaseUrl, config.Fetcher);
    });

export function setHost(url) {
    host = url;
}

export function syncInfo() {
    return client.syncInfo(null);
    //return http.get(host + '/edity/Git/SyncInfo', cacheBuster);
}

export function uncommittedChanges() {
    return client.uncommittedChanges(null);
    //return http.get(host + '/edity/Git/UncommittedChanges', cacheBuster);
}

export function commit(data: edityClient.NewCommit) {
    return client.commit(data, null);
    //return http.post(host + '/edity/Git/Commit', data);
}

export function uncommittedDiff(file:string) {
    return client.uncommittedDiff(file, null);
    //return http.get(host + '/edity/Git/UncommittedDiff/' + file, cacheBuster);
}

export function mergeInfo(file:string) {
    return client.mergeInfo(file, null);
    //return http.get(host + '/edity/Git/MergeInfo/' + file, cacheBuster);
}

export function historyCount(file, page, count) {
    client.fileHistory(file, page, count, null);
    //return http.get(host + '/edity/Git/HistoryCount/' + file, cacheBuster);
}

export function createHistoryPager(file, count) {
    return new PagedData(host + '/edity/Git/History/' + file, count);
}

export function resolve(file, content) {
    var blob = new Blob([content], { type: "text/html" });
    return uploader.upload(host + '/edity/Git/Resolve/' + file, blob);
}

export function pull() {
    return client.pull(null);
    //return http.post(host + '/edity/Git/Pull');
}

export function push() {
    return client.push(null);
    //return http.post(host + '/edity/Git/Push');
}

var revertStartedHandler = new EventHandler();
var revertCompletedHandler = new EventHandler();

export function revert(file) {
    revertStartedHandler.fire();
    return client.revert(file, null)
        .then((data) => {
            revertCompletedHandler.fire(true);
        })
        .catch((err) => {
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