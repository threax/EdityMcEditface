"use strict";

jsns.run([
    "hr.storage",
    "hr.controller",
    "hr.widgets.navmenu",
    "edity.GitService"
],
function (exports, module, storage, controller, navmenu, GitService) {
    function MergeController(bindings) {
        function MergeRow(bindings, data) {
            function merge(evt) {
                evt.preventDefault();
                dialog.on();

                GitService.mergeInfo(data.filePath)
                    .then(function (successData) {
                        initUI(data.filePath, successData);
                    })
                    .catch(function (failData) {
                        alert("Cannot read merge data, please try again later");
                    });
            }
            this.merge = merge;

            bindings.setListener(this);
        }

        function mergeRowCreated(bindings, data) {
            new MergeRow(bindings, data);
        }

        function mergeVariant(data) {
            if (data.state === "Conflicted") {
                return {
                    variant: "Conflicted",
                    rowCreated: mergeRowCreated
                };
            }
        }

        GitService.determineCommitVariantEvent.add(this, mergeVariant)

        var dialog = bindings.getToggle('dialog');
        var mergeModel = bindings.getModel('merge');
        var dv;
        var savePath;

        function initUI(path, data) {
            savePath = path;
            var target = document.getElementById("mergeViewArea");
            target.innerHTML = "";
            dv = CodeMirror.MergeView(target, {
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

        function save(evt) {
            evt.preventDefault();

            var content = dv.editor().getValue();
            GitService.resolve(savePath, content)
            .then(function (data) {
                dialog.off();
            })
            .catch(function (data) {
                alert("Error saving merge. Please try again later.");
            });
        }
        this.save = save;
    }

    controller.create("merge", MergeController);
});