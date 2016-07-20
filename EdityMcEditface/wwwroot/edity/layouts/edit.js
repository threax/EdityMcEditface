"use strict";

jsns.run(function (using) {
    var storage = using("htmlrest.storage");
    var component = using("htmlrest.components");

    var navMenu = Sizzle('[data-editor-navmenu]')[0];
    var navItems = Sizzle('[data-editor-navitem]');
    navItems.forEach(function (item) {
        navMenu.appendChild(item);
        item.style.display = "";
    });

    var navItems = storage.getInInstance("edit-nav-menu-items", []);
    navItems.forEach(function (item) {
        component.single(item.name, null, navMenu, item.created);
    });
});