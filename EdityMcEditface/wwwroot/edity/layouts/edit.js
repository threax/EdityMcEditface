"use strict";

jsns.define("edity.pageSourceSync", [

], function (exports, module) {
    var sourceAccessor;

    function setHtml(value) {
        sourceAccessor.setHtml(value);
    }
    exports.setHtml = setHtml;

    function getHtml() {
        return sourceAccessor.getHtml();
    }
    exports.getHtml = getHtml;

    function setSourceAccessor(value) {
        sourceAccessor = value;
    }
    exports.setSourceAccessor = setSourceAccessor;
});

jsns.run([
    "htmlrest.storage",
    "htmlrest.components",
    "htmlrest.domquery",
    "htmlrest.widgets.navmenu"
],
function(exports, module, storage, component, domQuery, navMenuWidget) {

    var navMenu = domQuery.first('[data-editor-navmenu]');

    var navItems = storage.getInInstance("edit-nav-menu-items", []);
    navItems.forEach(function (item) {
        component.single(item.name, navMenu, null, item.created);
    });

    var menu = navMenuWidget.getNavMenu("edit-nav-menu-items");
    menu.getItems().forEach(function (item) {
        component.single(item.name, navMenu, null, item.created);
    });
});