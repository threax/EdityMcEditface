"use strict";

jsns.define("edity.CompileService", [
    "hr.http",
    "hr.eventhandler"
], function (exports, module, http, EventHandler) {
    var host = "";

    function setHost(url) {
        host = url;
    }
    exports.setHost = setHost;

    function getStatus() {
        return http.get(host + '/edity/Compile/Status');
    }
    exports.getStatus = getStatus;
});