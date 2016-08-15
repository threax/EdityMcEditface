"use strict";

jsns.define("edity.GitService", [
    "htmlrest.rest",
    "htmlrest.eventhandler",
    "htmlrest.data.paged",
], function (exports, module, rest, EventHandler, PagedData) {
    var host = "";

    function setHost(url) {
        host = url;
    }
    exports.setHost = setHost;

    function syncInfo() {
        return rest.getPromise(host + '/edity/Git/SyncInfo');
    }
    exports.syncInfo = syncInfo;

    function uncommittedChanges() {
        return rest.getPromise(host + '/edity/Git/UncommittedChanges');
    }
    exports.uncommittedChanges = uncommittedChanges;

    function commit(data) {
        return rest.postPromise(host + '/edity/Git/Commit', data);
    }
    exports.commit = commit;

    function uncommittedDiff(file) {
        return rest.getPromise(host + '/edity/Git/UncommittedDiff/' + file);
    }
    exports.uncommittedDiff = uncommittedDiff;

    function mergeInfo(file) {
        return rest.getPromise(host + '/edity/Git/MergeInfo/' + file);
    }
    exports.mergeInfo = mergeInfo;

    function historyCount(file) {
        return rest.getPromise(host + '/edity/Git/HistoryCount/' + file);
    }
    exports.historyCount = historyCount;

    function createHistoryPager(file, count) {
        return new PagedData(host + '/edity/Git/History/' + file, count);
    }
    exports.createHistoryPager = createHistoryPager;

    function resolve(file, content) {
        var blob = new Blob([content], { type: "text/html" });
        return rest.uploadPromise(host + '/edity/Git/Resolve/' + file, blob);
    }
    exports.resolve = resolve;

    function pull() {
        return rest.postPromise(host + '/edity/Git/Pull');
    }
    exports.pull = pull;

    function push() {
        return rest.postPromise(host + '/edity/Git/Push');
    }
    exports.push = push;

    var revertStarted = new EventHandler();
    var revertCompleted = new EventHandler();

    function revert(file) {
        revertStarted.fire();
        return rest.postPromise(host + '/edity/Git/Revert/' + file)
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