"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.toggles",
    "htmlrest.widgets.navmenu",
    "edity.CompileService"
],
function (exports, module, storage, rest, controller, toggles, navmenu, CompileService) {

    function CompileController(bindings) {
        var start = bindings.getToggle("start");
        var success = bindings.getToggle("success");
        var fail = bindings.getToggle("fail");
        var compiling = bindings.getToggle("compiling");
        var toggleGroup = new toggles.Group(start, success, fail, compiling);
        var resultsModel = bindings.getModel("results");
        var changesModel = bindings.getModel("changes");
        var infoModel = bindings.getModel('info');

        var publishToggle = bindings.getToggle('publish');

        var dialogToggle = bindings.getToggle('dialog');

        bindings.setListener({
            runCompiler: function (evt) {
                evt.preventDefault();
                publishToggle.off();
                toggleGroup.show(compiling);
                rest.post('/edity/Compile', {},
                    function (data) {
                        publishToggle.on();
                        resultsModel.setData(data);
                        toggleGroup.show(success);
                    }, function () {
                        publishToggle.on();
                        toggleGroup.show(fail);
                    });
            }
        });

        function NavButtonController(created) {
            function compile() {
                toggleGroup.show(compiling);
                dialogToggle.on();
                CompileService.getStatus()
                .then(function (data) {
                    infoModel.setData(data);
                    changesModel.setData(data.behindHistory);
                    toggleGroup.activate(start);
                })
                .catch(function (err) {
                    toggleGroup.show(fail);
                });
            }
            this.compile = compile;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("CompileNavItem", NavButtonController);
    }

    controller.create('compile', CompileController);
});