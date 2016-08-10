"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu",
    "edity.commitSync"
],
function (exports, module, storage, rest, controller, navmenu, commitSync) {
    function MergeController(bindings) {
        function MergeRow(bindings, data) {
            function merge(evt) {
                evt.preventDefault();
                dialog.on();

                rest.get(source + '/' + data.filePath, function (successData) {
                    initUI(data.filePath, successData);
                },
                function (failData) {
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

        commitSync.determineCommitVariantEvent.add(this, mergeVariant)

        var dialog = bindings.getToggle('dialog');
        var mergeModel = bindings.getModel('merge');
        var source = mergeModel.getSrc();
        var config = bindings.getConfig();
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
            var blob = new Blob([content], { type: "text/html" });
            rest.upload(config.resolvepath + '/' + savePath, blob, function () {
                dialog.off();
            },
            function () {
                alert("Error saving merge. Please try again later.");
            });
        }
        this.save = save;
    }

    controller.create("merge", MergeController);
});