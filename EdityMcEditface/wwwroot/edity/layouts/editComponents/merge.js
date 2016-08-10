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
                    initUI(successData);
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
    }

    controller.create("merge", MergeController);
});