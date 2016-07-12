"use strict";

(function (s, h) {
    var navMenu = s('[data-editor-navmenu]')[0];
    var navItems = s('[data-editor-navitem]');
    navItems.forEach(function (item) {
        navMenu.appendChild(item);
        item.style.display = "";
    });

    var navItems = h.storage.getInInstance("edit-nav-menu-items", []);
    navItems.forEach(function (item) {
        h.createComponent(item.name, null, navMenu, item.created);
    });
})(Sizzle, htmlrest);