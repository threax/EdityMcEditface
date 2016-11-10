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
}

export function uncommittedChanges() {
    return client.uncommittedChanges(null);
}

export function commit(data: edityClient.NewCommit) {
    return client.commit(data, null);
}

export function uncommittedDiff(file:string) {
    return client.uncommittedDiff(file, null);
}

export function mergeInfo(file:string) {
    return client.mergeInfo(file, null);
}

export function historyCount(file) {
    return client.repoHistoryCount(file, null);
}

export function createHistoryPager(file, count) {
    return new PagedData(host + '/edity/Git/History/' + file, count);
}

export function resolve(file, content) {
    var blob = new Blob([content], { type: "text/html" });
    return client.resolve(file, { data: blob, fileName: file }, null);
}

export function pull() {
    return client.pull(null);
}

export function push() {
    return client.push(null);
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