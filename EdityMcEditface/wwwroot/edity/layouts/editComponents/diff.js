"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "edity.commitSync"
],
function (exports, module, storage, rest, controller, navmenu, commitSync) {
    function DiffController(bindings) {
        function DiffRow(bindings, data) {
            function diff(evt) {
                evt.preventDefault();
                dialog.on();

                rest.get(source + '/' + data.filePath, function (successData) {
                    initUI(successData);
                },
                function (failData) {
                    alert("Cannot read diff data, please try again later");
                });
            }
            this.diff = diff;

            bindings.setListener(this);
        }

        function diffRowCreated(bindings, data) {
            new DiffRow(bindings, data);
        }

        function diffVariant(data) {
            if (data.state === "Modified") {
                return {
                    variant: "ModifiedWithDiff",
                    rowCreated: diffRowCreated
                };
            }
        }

        commitSync.determineCommitVariantEvent.add(this, diffVariant)

        var dialog = bindings.getToggle('dialog');
        var diffModel = bindings.getModel('diff');
        var source = diffModel.getSrc();

        function initUI(data) {
            var target = document.getElementById("diffViewArea");
            target.innerHTML = "";
            var dv = CodeMirror.MergeView(target, {
                value: data.changed,
                origLeft: data.original,
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
            dv.wrap.style.height = height + "px";
            setTimeout(function () {
                dv.editor().refresh();
                dv.leftOriginal().refresh();
            }, 500);
        }
    }

    controller.create("diff", DiffController);
});