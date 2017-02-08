"use strict";

import * as navmenu from "edity.editorcore.navmenu";
import * as pageService from "edity.editorcore.PageService";
import * as saveService from "edity.editorcore.SaveService";
import * as controller from 'hr.controller';
import * as PageStart from 'edity.editorcore.EditorPageStart';

class SaveController {
    private load: controller.OnOffToggle;

    constructor(bindings: controller.BindingCollection){
        this.load = bindings.getToggle("load");
        this.load.off();

        saveService.saveStartedEvent.add(() => this.load.on());
        saveService.saveCompletedEvent.add(() => this.load.off());
        saveService.saveErrorEvent.add(() => this.load.off());
        saveService.saveErrorEvent.add(() => this.saveError());
    }

    private saveError() {
        alert('Error saving changes. Please try again later.')
    }

    save(evt) {
        evt.preventDefault();
        saveService.saveNow();
    }
}

PageStart.init().then(config => {
    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("SaveButton", SaveController);
});