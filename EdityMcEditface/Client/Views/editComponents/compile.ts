"use strict";

import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as toggles from "hr.toggles";
import * as navmenu from "edity.editorcore.navmenu";
import * as EdityClient from 'edity.editorcore.EdityClient';
import * as PageStart from 'edity.editorcore.EditorPageStart';
import * as CompileService from 'edity.editorcore.CompileService';

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
    private toggleGroup: toggles.Group;
    private resultsModel;
    private changesModel;
    private infoModel;
    private dialogToggle;
    private compileClient: EdityClient.CompileClient;
    private compileService: CompileService.CompilerService;

    constructor(bindings: controller.BindingCollection, context: PageStart.EditorPageStart) {
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
        this.compileService = context.CompilerService;
        this.compileService.onStarted.add(a => this.toggleGroup.activate(this.compiling));
        this.compileService.onStatusUpdated.add(a => this.statusUpdated(a));
        this.compileService.onFailed.add(a => this.toggleGroup.activate(this.fail));
        this.compileService.onSuccess.add(a => this.toggleGroup.activate(this.success));
    }

    runCompiler(evt) {
        evt.preventDefault();
        this.compileService.compile();
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

    private statusUpdated(arg: CompileService.CompilerServiceStatusEventArgs){

    }
}

var compileController;

PageStart.init().then((config) => {
    compileController = controller.create<CompileController, PageStart.EditorPageStart, void>('compile', CompileController, config)[0];
});