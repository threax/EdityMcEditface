"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "edity.commitSync",
    "edity.GitService"
],
function (exports, module, storage, rest, controller, navmenu, commitSync, GitService) {
    function DiffController(bindings) {
        function DiffRow(bindings, data) {
            function diff(evt) {
                evt.preventDefault();
                dialog.on();

                GitService.uncommittedDiff(data.filePath)
                .then(function (successData) {
                    initUI(data.filePath, successData);
                })
                .catch(function (failData) {
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
        var dv;
        var config = bindings.getConfig();
        var savePath;

        function initUI(path, data) {
            savePath = path;
            var target = document.getElementById("diffViewArea");
            target.innerHTML = "";
            dv = CodeMirror.MergeView(target, {
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

        function save(evt) {
            evt.preventDefault();

            var content = dv.editor().getValue();
            var blob = new Blob([content], { type: "text/html" });
            rest.upload(config.saveurl + '/' + savePath, blob, function () {
                dialog.off();
            },
            function () {
                alert("Error saving merge. Please try again later.");
            });
        }
        this.save = save;
    }

    controller.create("diff", DiffController);
});