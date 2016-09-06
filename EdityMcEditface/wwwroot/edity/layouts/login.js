"use strict";

jsns.run([
    "hr.http",
    "hr.controller",
    "hr.uri"
],
function (exports, module, http, controller, uri) {
    function LoginController(bindings) {
        var loginModel = bindings.getModel('login');

        var errorModel = bindings.getModel('error');
        var errorToggle = bindings.getToggle('error');

        function login(evt) {
            evt.preventDefault();
            errorToggle.off();
            var data = loginModel.getData();
            http.post('/edity/Auth/LogIn', data)
            .then(function (res) {
                var queryObj = uri.getQueryObject();
                var url = uri.parseUri(queryObj.returnurl);
                window.location.href = url.relative;
            })
            .catch(function (err) {
                errorToggle.on();
                if (!err.message) {
                    err.message = "Unknown error";
                }
                errorModel.setData(err);
            });
        }
        this.login = login;
    }

    controller.create("login", LoginController);
});