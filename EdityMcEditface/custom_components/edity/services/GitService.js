"use strict";

jsns.define("edity.GitService", [
    "hr.http",
    "hr.eventhandler",
    "hr.data.paged",
], function (exports, module, http, EventHandler, PagedData) {
    var host = "";

    function setHost(url) {
        host = url;
    }
    exports.setHost = setHost;

    function syncInfo() {
        return http.get(host + '/edity/Git/SyncInfo');
    }
    exports.syncInfo = syncInfo;

    function uncommittedChanges() {
        return http.get(host + '/edity/Git/UncommittedChanges');
    }
    exports.uncommittedChanges = uncommittedChanges;

    function commit(data) {
        return http.post(host + '/edity/Git/Commit', data);
    }
    exports.commit = commit;

    function uncommittedDiff(file) {
        return http.get(host + '/edity/Git/UncommittedDiff/' + file);
    }
    exports.uncommittedDiff = uncommittedDiff;

    function mergeInfo(file) {
        return http.get(host + '/edity/Git/MergeInfo/' + file);
    }
    exports.mergeInfo = mergeInfo;

    function historyCount(file) {
        return http.get(host + '/edity/Git/HistoryCount/' + file);
    }
    exports.historyCount = historyCount;

    function createHistoryPager(file, count) {
        return new PagedData(host + '/edity/Git/History/' + file, count);
    }
    exports.createHistoryPager = createHistoryPager;

    function resolve(file, content) {
        var blob = new Blob([content], { type: "text/html" });
        return http.upload(host + '/edity/Git/Resolve/' + file, blob);
    }
    exports.resolve = resolve;

    function pull() {
        return http.post(host + '/edity/Git/Pull');
    }
    exports.pull = pull;

    function push() {
        return http.post(host + '/edity/Git/Push');
    }
    exports.push = push;

    var revertStarted = new EventHandler();
    var revertCompleted = new EventHandler();

    function revert(file) {
        revertStarted.fire();
        return http.post(host + '/edity/Git/Revert/' + file)
        .then(function (data) {
            revertCompleted.fire(true);
        })
        .catch(function (data) {
            revertCompleted.fire(false);
        });
    }
    exports.revert = revert;
    exports.revertStarted = revertStarted.modifier;
    exports.revertCompleted = revertCompleted.modifier;

    //Commit variant detection and sync
    var determineCommitVariantEvent = new EventHandler();

    function fireDetermineCommitVariant(data) {
        return determineCommitVariantEvent.fire(data);
    }

    exports.determineCommitVariantEvent = determineCommitVariantEvent.modifier;
    exports.fireDetermineCommitVariant = fireDetermineCommitVariant;
});