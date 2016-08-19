"use strict";

jsns.run([
    "hr.widgets.navmenu",
    "edity.PageService",
    "edity.SaveService"
],
function (exports, module, navmenu, pageService, saveService) {
    function SaveController(component) {
        var load = component.getToggle("load");
        load.off();

        saveService.saveStartedEvent.add(load, load.on);
        saveService.saveCompletedEvent.add(load, load.off);
        saveService.saveErrorEvent.add(load, load.off);

        function save(evt) {
            evt.preventDefault();
            saveService.saveNow();
        }
        this.save = save;
    }

    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("SaveButton", SaveController);
    editMenu.add("PreviewButton");
});