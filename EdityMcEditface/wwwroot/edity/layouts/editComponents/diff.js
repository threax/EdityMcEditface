"use strict";

jsns.run([
    "hr.storage",
    "hr.http",
    "hr.controller",
    "hr.widgets.navmenu",
    "edity.GitService"
],
function (exports, module, storage, http, controller, navmenu, GitService) {
    var revertConfirmation;

    function ConfirmRevertController(bindings) {
        revertConfirmation = this;
        var targetFile;
        var dialog = bindings.getToggle('dialog');
        var info = bindings.getModel('info');

        function revert() {
            GitService.revert(targetFile)
            .then(function (data) {
                dialog.off();
            });
            targetFile = null;
        }
        this.revert = revert;

        function confirm(file) {
            targetFile = file;
            info.setData(file);
            dialog.on();
        }
        this.confirm = confirm;
    }
    controller.create("diff-revertFileConfirmation", ConfirmRevertController);

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

            function revert(evt) {
                evt.preventDefault();

                revertConfirmation.confirm(data.filePath);
            }
            this.revert = revert;

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

        GitService.determineCommitVariantEvent.add(this, diffVariant)

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
            http.upload(config.saveurl + '/' + savePath, blob)
            .then(function () {
                dialog.off();
            })
            .catch(function () {
                alert("Error saving merge. Please try again later.");
            });
        }
        this.save = save;
    }

    controller.create("diff", DiffController);
});