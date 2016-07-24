﻿"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.components",
    "htmlrest.domquery"
],
function(exports, module, storage, component, domQuery) {

    var navMenu = domQuery.first('[data-editor-navmenu]');

    var navItems = storage.getInInstance("edit-nav-menu-items", []);
    navItems.forEach(function (item) {
        component.single(item.name, navMenu, null, item.created);
    });
});