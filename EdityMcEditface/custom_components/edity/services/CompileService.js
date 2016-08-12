"use strict";

jsns.define("edity.CompileService", [
    "htmlrest.rest",
    "htmlrest.eventhandler"
], function (exports, module, rest, EventHandler) {
    var host = "";

    function setHost(url) {
        host = url;
    }
    exports.setHost = setHost;

    function getStatus() {
        return rest.getPromise(host + '/edity/Compile/Status');
    }
    exports.getStatus = getStatus;
});