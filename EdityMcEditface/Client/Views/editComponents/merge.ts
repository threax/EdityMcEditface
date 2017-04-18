///<amd-module name="edity.core.edit.components.merge"/>

"use strict";

import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as navmenu from "edity.editorcore.navmenu";
import * as GitService from "edity.editorcore.GitService";

var CodeMirror = (<any>window).CodeMirror;

class MergeRow {
    private mergeController;
    private data;

    constructor(bindings: controller.BindingCollection, context: MergeController, data) {
        bindings.setListener(this);
        this.mergeController = context;
        this.data = data;
    }

    merge(evt) {
        evt.preventDefault();
        this.mergeController.startUi();

        GitService.mergeInfo(this.data.filePath)
            .then((successData) => this.mergeController.initUI(this.data.filePath, successData))
            .catch((failData) => alert("Cannot read merge data, please try again later"));
    }
}

class MergeController {
    private dialog;
    private mergeModel;
    private dv;
    private savePath;

    constructor(bindings: controller.BindingCollection) {
        this.dialog = bindings.getToggle('dialog');
        this.mergeModel = bindings.getModel('merge');

        GitService.determineCommitVariantEvent.add((d) => this.mergeVariant(d));
    }

    private mergeVariant(data) {
        if (data.state === "Conflicted") {
            return {
                variant: "Conflicted",
                rowCreated: (bindings, data) => new MergeRow(bindings, this, data)
            };
        }
    }

    startUi() {
        this.dialog.on();
    }

    initUI(path, data) {
        this.savePath = path;
        var target = document.getElementById("mergeViewArea");
        target.innerHTML = "";
        this.dv = CodeMirror.MergeView(target, {
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
        this.dv.editor().setSize(null, height);
        this.dv.leftOriginal().setSize(null, height);
        this.dv.rightOriginal().setSize(null, height);
        this.dv.wrap.style.height = height + "px";
        setTimeout(() => {
            this.dv.editor().refresh();
            this.dv.leftOriginal().refresh();
            this.dv.rightOriginal().refresh();
        }, 500);
    }

    save(evt) {
        evt.preventDefault();

        var content = this.dv.editor().getValue();
        GitService.resolve(this.savePath, content)
            .then((data) => this.dialog.off())
            .catch((data) => alert("Error saving merge. Please try again later."));
    }
}

controller.create<MergeController, void, void>("merge", MergeController);