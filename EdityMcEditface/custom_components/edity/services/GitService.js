"use strict";

jsns.define("edity.GitService", [
    "htmlrest.rest"
], function (exports, module, rest) {
    var host = "http://localhost:5000";

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
});