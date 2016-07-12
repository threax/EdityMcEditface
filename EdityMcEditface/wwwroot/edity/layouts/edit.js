"use strict";

(function (s, h) {
    var navMenu = s('[data-editor-navmenu]')[0];
    var navItems = s('[data-editor-navitem]');
    navItems.forEach(function (item) {
        navMenu.appendChild(item);
        item.style.display = "";
    });
    
})(Sizzle, htmlrest);