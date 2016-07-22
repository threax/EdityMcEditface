"use strict";

jsns.run(function (using) {
    var storage = using("htmlrest.storage");
    var rest = using("htmlrest.rest");
    var BindingCollection = using("htmlrest.bindingcollection");
    var toggles = using("htmlrest.toggles");

    var bindings = new BindingCollection("#compileModal");

    var start = bindings.getToggle("start");
    var success = bindings.getToggle("success");
    var fail = bindings.getToggle("fail");
    var compiling = bindings.getToggle("compiling");
    var toggleGroup = new toggles.group(start, success, fail, compiling);

    var publishToggle = bindings.getToggle('publish');

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

    //var compileButton = $('#CompileButton');

    //compileButton.click(function () {
    //    compileButton.prop('disabled', true);
    //    compilerOutputMessage.compiling();
    //    rest.post('/edity/Compile', {},
    //    function () {
    //        compileButton.prop('disabled', false);
    //        compilerOutputMessage.succeeded();
    //    }, function () {
    //        compileButton.prop('disabled', false);
    //        compilerOutputMessage.failed();
    //    });
    //    return false;
    //});

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "CompileNavItem",
        created: function (button) {
            button.setListener({
                compile: function () {
                    toggleGroup.show(start);
                }
            });
        }});
});