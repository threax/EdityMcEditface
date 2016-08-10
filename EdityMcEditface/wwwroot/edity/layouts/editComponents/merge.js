"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu"
],
function (exports, module, storage, rest, controller, navmenu) {
    function MergeController(bindings) {
        var dialog = bindings.getToggle('dialog');
        var mergeModel = bindings.getModel('merge');
        var source = mergeModel.getSrc();

        function initUI(data) {
            var target = document.getElementById("mergeViewArea");
            target.innerHTML = "";
            var dv = CodeMirror.MergeView(target, {
                value: data.merged,
                origLeft: data.theirs,
                orig: data.mine,
                lineNumbers: true,
                mode: "text/html",
                highlightDifferences: true,
                connect: true,
                collapseIdentical: true,
                theme: "edity"
            });

            var height = window.innerHeight - 250;
            dv.editor().setSize(null, height);
            dv.leftOriginal().setSize(null, height);
            dv.rightOriginal().setSize(null, height);
            dv.wrap.style.height = height + "px";
            setTimeout(function () {
                dv.editor().refresh();
                dv.leftOriginal().refresh();
                dv.rightOriginal().refresh();
            }, 500);
        }

        function NavButtonController(created) {
            function merge() {
                dialog.on();
                rest.get(source + window.location.pathname, function (data) {
                    initUI(data);
                },
                function (failData) {
                    alert("Cannot read merge data, please try again later");
                })
            }
            this.merge = merge;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("MergeNavItem", NavButtonController);
    }

    controller.create("merge", MergeController);
});