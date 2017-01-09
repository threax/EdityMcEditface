"use strict";

import * as component from "hr.components";
import * as domQuery from "hr.domquery";
import * as navmenu from "edity.editorcore.navmenu";

var navMenu = domQuery.first('[data-editor-navmenu]');

function itemAdded(item) {
    component.single(item.name, navMenu, null, item.created);
}

var menu = navmenu.getNavMenu("edit-nav-menu-items");
menu.getItems().forEach(itemAdded);
menu.itemAdded.add(itemAdded);