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
    "htmlrest.components",
    "htmlrest.domquery",
    "htmlrest.widgets.navmenu"
],
function (exports, module, component, domQuery, navmenu) {
    var navMenu = domQuery.first('[data-editor-navmenu]');

    function itemAdded(item) {
        component.single(item.name, navMenu, null, item.created);
    }

    var menu = navmenu.getNavMenu("edit-nav-menu-items");
    menu.getItems().forEach(itemAdded);
    menu.itemAdded.add(null, itemAdded);
});