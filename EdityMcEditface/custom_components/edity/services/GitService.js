"use strict";

jsns.define("edity.GitService", [
    "htmlrest.rest"
], function (exports, module, rest) {
    var host = "http://localhost:5000";

    function setHost(url) {
        host = url;
    }
    exports.setHost = setHost;

    function hasUncommittedChanges() {
        return rest.getPromise(host + '/edity/Git/HasUncommittedChanges');
    }
    exports.hasUncommittedChanges = hasUncommittedChanges;

    function uncommittedChanges() {
        return rest.getPromise(host + '/edity/Git/UncommittedChanges');
    }
    exports.uncommittedChanges = uncommittedChanges;

    function commit(data) {
        return rest.postPromise(host + '/edity/Git/Commit', data);
    }
    exports.commit = commit;
});