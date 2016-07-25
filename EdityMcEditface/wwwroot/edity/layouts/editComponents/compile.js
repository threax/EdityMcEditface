﻿"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.toggles"
],
function (exports, module, storage, rest, controller, toggles) {

    function CompileController(bindings) {
        var start = bindings.getToggle("start");
        var success = bindings.getToggle("success");
        var fail = bindings.getToggle("fail");
        var compiling = bindings.getToggle("compiling");
        var toggleGroup = new toggles.Group(start, success, fail, compiling);

        var publishToggle = bindings.getToggle('publish');

        var dialogToggle = bindings.getToggle('dialog');

        bindings.setListener({
            runCompiler: function (evt) {
                evt.preventDefault();
                publishToggle.off();
                toggleGroup.show(compiling);
                rest.post('/edity/Compile', {},
                    function () {
                        publishToggle.on();
                        toggleGroup.show(success);
                    }, function () {
                        publishToggle.on();
                        toggleGroup.show(fail);
                    });
            }
        });

        var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
        buttonCreation.push({
            name: "CompileNavItem",
            created: function (button) {
                button.setListener({
                    compile: function () {
                        toggleGroup.show(start);
                        dialogToggle.on();
                    }
                });
            }
        });
    }

    controller.create('compile', CompileController);
});