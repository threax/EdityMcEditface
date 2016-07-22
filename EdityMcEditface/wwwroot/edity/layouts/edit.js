"use strict";

jsns.run(function (using) {
    var storage = using("htmlrest.storage");
    var component = using("htmlrest.components");

    var navMenu = $('[data-editor-navmenu]')[0];

    var navItems = storage.getInInstance("edit-nav-menu-items", []);
    navItems.forEach(function (item) {
        component.single(item.name, navMenu, null, item.created);
    });
});