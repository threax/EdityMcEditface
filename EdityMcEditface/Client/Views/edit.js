"use strict";

jsns.run([
    "hr.components",
    "hr.domquery",
    "hr.widgets.navmenu"
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