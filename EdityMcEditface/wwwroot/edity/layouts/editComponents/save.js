"use strict";

jsns.run([
    "htmlrest.rest",
    "htmlrest.widgets.navmenu",
    "edity.pageSourceSync"
],
function (exports, module, rest, navmenu, sourceSync) {
    function SaveController(component) {
        var load = component.getToggle("load");
        load.off();

        function save(evt) {
            var saveModel = component.getModel("save");

            evt.preventDefault();

            load.on();
            var content = sourceSync.getHtml();
            var blob = new Blob([content], { type: "text/html" });
            rest.upload(saveModel.getSrc() + '/' + window.location.pathname, blob, function () {
                load.off();
            },
            function () {
                load.off();
                alert("Error saving page. Please try again later.");
            });
        }
        this.save = save;
    }

    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("SaveButton", SaveController);
    editMenu.add("PreviewButton");
});