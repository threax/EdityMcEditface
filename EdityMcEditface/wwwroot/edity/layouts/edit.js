"use strict";

jsns.run(function (using) {
    var storage = using("htmlrest.storage");
    var component = using("htmlrest.components");
    var domQuery = using("htmlrest.domquery");

    var navMenu = domQuery.first('[data-editor-navmenu]');

    var navItems = storage.getInInstance("edit-nav-menu-items", []);
    navItems.forEach(function (item) {
        component.single(item.name, navMenu, null, item.created);
    });
});