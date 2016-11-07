"use strict";

import * as storage from "hr.storage";
import * as http from "hr.http";
import * as controller from "hr.controller";
import * as toggles from "hr.toggles";
import * as navmenu from "hr.widgets.navmenu";
import * as CompileService from "clientlibs.CompileService";

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

    constructor(bindings: controller.BindingCollection) {
        this.start = bindings.getToggle("start");
        this.success = bindings.getToggle("success");
        this.fail = bindings.getToggle("fail");
        this.compiling = bindings.getToggle("compiling");
        this.toggleGroup = new toggles.Group(this.start, this.success, this.fail, this.compiling);
        this.resultsModel = bindings.getModel("results");
        this.changesModel = bindings.getModel("changes");
        this.infoModel = bindings.getModel('info');
        this.dialogToggle = bindings.getToggle('dialog');
    }

    runCompiler(evt) {
        evt.preventDefault();
        this.toggleGroup.activate(this.compiling);
        http.post('/edity/Compile', {})
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
        CompileService.getStatus()
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

var compileController = controller.create<CompileController, void, void>('compile', CompileController)[0];