"use strict";

jsns.run([
    "hr.anticsrf"
], function (exports, module, anticsrf) {
    anticsrf.getToken("/edity/Auth/AntiforgeryToken");
});