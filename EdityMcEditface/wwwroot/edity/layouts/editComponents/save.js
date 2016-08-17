"use strict";

jsns.run([
    "htmlrest.rest",
    "htmlrest.widgets.navmenu",
    "edity.PageService"
],
function (exports, module, rest, navmenu, pageService) {
    function SaveController(component) {
        var load = component.getToggle("load");
        load.off();

        function save(evt) {
            var saveModel = component.getModel("save");

            evt.preventDefault();

            load.on();
            var content = pageService.getHtml();
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