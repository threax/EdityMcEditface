"use strict";

import * as edityClient from 'edity.editorcore.EdityClient';
import * as uploader from 'edity.editorcore.uploader';
import { ActionEventDispatcher } from 'hr.eventdispatcher';
import { PagedClientData } from 'edity.editorcore.pageddata';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import * as pageStart from 'edity.editorcore.PageStart';

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
    return new PagedClientData<edityClient.History[]>((current, resultsPerPage) => getDataPage(file, current, resultsPerPage), count);
}

function getDataPage(file, current, resultsPerPage): Promise<edityClient.History[]> {
    return client.fileHistory(file, current, resultsPerPage, null);
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

var revertStartedHandler = new ActionEventDispatcher<void>();
var revertCompletedHandler = new ActionEventDispatcher<boolean>();

export function revert(file) {
    revertStartedHandler.fire(null);
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
var determineCommitVariantEventHandler = new ActionEventDispatcher<any>();

export function fireDetermineCommitVariant(data) {
    return determineCommitVariantEventHandler.fire(data);
}

export const determineCommitVariantEvent = determineCommitVariantEventHandler.modifier;