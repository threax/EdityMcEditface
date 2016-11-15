"use strict";

import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as toggles from "hr.toggles";
import * as navmenu from "hr.widgets.navmenu";
import * as EdityClient from 'edity.editorcore.EdityClient';
import * as PageStart from 'edity.editorcore.PageStart';

class NavButtonController {
    compile(evt) {
        evt.preventDefault();
        compileController.startCompile();
    }
}

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.add("CompileNavItem", NavButtonController);

class CompileController {
    private start;
    private success;
    private fail;
    private compiling;
    private toggleGroup;
    private resultsModel;
    private changesModel;
    private infoModel;
    private dialogToggle;
    private compileClient: EdityClient.CompileClient;

    constructor(bindings: controller.BindingCollection, context: PageStart.PageStart) {
        this.start = bindings.getToggle("start");
        this.success = bindings.getToggle("success");
        this.fail = bindings.getToggle("fail");
        this.compiling = bindings.getToggle("compiling");
        this.toggleGroup = new toggles.Group(this.start, this.success, this.fail, this.compiling);
        this.resultsModel = bindings.getModel("results");
        this.changesModel = bindings.getModel("changes");
        this.infoModel = bindings.getModel('info');
        this.dialogToggle = bindings.getToggle('dialog');
        this.compileClient = new EdityClient.CompileClient(context.BaseUrl, context.Fetcher);
    }

    runCompiler(evt) {
        evt.preventDefault();
        this.toggleGroup.activate(this.compiling);
        this.compileClient.compile(null)
            .then((data) => {
                this.resultsModel.setData(data);
                this.toggleGroup.activate(this.success);
            })
            .catch(() => {
                this.toggleGroup.activate(this.fail);
            });
    }

    startCompile() {
        this.toggleGroup.activate(this.compiling);
        this.dialogToggle.on();
        this.compileClient.status(null)
            .then((data: any) => {
                this.infoModel.setData(data);
                this.changesModel.setData(data.behindHistory);
                this.toggleGroup.activate(this.start);
            })
            .catch((err) => {
                this.toggleGroup.activate(this.fail);
            });
    }
}

var compileController;

PageStart.init().then((config) => {
    compileController = controller.create<CompileController, PageStart.PageStart, void>('compile', CompileController, config)[0];
});