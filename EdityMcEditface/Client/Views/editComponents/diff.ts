"use strict";

import * as storage from "hr.storage";
import * as uploader from "edity.editorcore.uploader";
import * as controller from "hr.controller";
import * as navmenu from "hr.widgets.navmenu";
import * as GitService from "edity.editorcore.GitService";

var revertConfirmation;
var CodeMirror = (<any>window).CodeMirror;

class ConfirmRevertController{
    private revertConfirmation = this;
    private targetFile;
    private dialog;
    private info;

    constructor(bindings: controller.BindingCollection) {
        this.revertConfirmation = this;
        this.targetFile;
        this.dialog = bindings.getToggle('dialog');
        this.info = bindings.getModel('info');
    }

    revert() {
        GitService.revert(this.targetFile)
        .then((data) => {
            this.dialog.off();
        });
        this.targetFile = null;
    }

    confirm(file) {
        this.targetFile = file;
        this.info.setData(file);
        this.dialog.on();
    }
}
revertConfirmation = controller.create("diff-revertFileConfirmation", ConfirmRevertController, null)[0];

class DiffRow {
    private data;

    constructor(bindings, data) {
        this.data = data;
        bindings.setListener(this);
    }

    diff(evt) {
        evt.preventDefault();
        diffControllerInstance.openDialog();

        GitService.uncommittedDiff(this.data.filePath)
            .then((successData) => {
                diffControllerInstance.initUI(this.data.filePath, successData);
            })
            .catch((failData) => {
                alert("Cannot read diff data, please try again later");
            });
    }

    revert(evt) {
        evt.preventDefault();

        revertConfirmation.confirm(this.data.filePath);
    }
}

class DiffController {
    private dialog;
    private diffModel;
    private dv;
    private config;
    private savePath;

    constructor(bindings) {
        GitService.determineCommitVariantEvent.add(this, this.diffVariant)

        this.dialog = bindings.getToggle('dialog');
        this.diffModel = bindings.getModel('diff');
        this.config = bindings.getConfig();
    }

    private diffVariant(data) {
        if (data.state === "Modified") {
            return {
                variant: "ModifiedWithDiff",
                rowCreated: (bindings, data) => new DiffRow(bindings, data)
            };
        }
    }

    initUI(path, data) {
        this.savePath = path;
        var target = document.getElementById("diffViewArea");
        target.innerHTML = "";
        this.dv = CodeMirror.MergeView(target, {
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
        this.dv.editor().setSize(null, height);
        this.dv.leftOriginal().setSize(null, height);
        this.dv.wrap.style.height = height + "px";
        setTimeout(() => {
            this.dv.editor().refresh();
            this.dv.leftOriginal().refresh();
        }, 500);
    }

    openDialog() {
        this.dialog.on();
    }

    save(evt) {
        evt.preventDefault();

        var content = this.dv.editor().getValue();
        var blob = new Blob([content], { type: "text/html" });
        uploader.upload(this.config.saveurl + '/' + this.savePath, blob)
        .then(function () {
            this.dialog.off();
        })
        .catch(function () {
            alert("Error saving merge. Please try again later.");
        });
    }
}

var diffControllerInstance = controller.create<DiffController, void, void>("diff", DiffController)[0];