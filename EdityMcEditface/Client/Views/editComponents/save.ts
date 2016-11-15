"use strict";

import * as navmenu from "hr.widgets.navmenu";
import * as pageService from "edity.editorcore.PageService";
import * as saveService from "edity.editorcore.SaveService";

class SaveController {
    constructor(bindings){
        var load = bindings.getToggle("load");
        load.off();

        saveService.saveStartedEvent.add(load, load.on);
        saveService.saveCompletedEvent.add(load, load.off);
        saveService.saveErrorEvent.add(load, load.off);
        saveService.saveErrorEvent.add(this, this.saveError);
    }

    private saveError() {
        alert('Error saving changes. Please try again later.')
    }

    save(evt) {
        evt.preventDefault();
        saveService.saveNow();
    }
}

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.add("SaveButton", SaveController);
editMenu.add("PreviewButton");