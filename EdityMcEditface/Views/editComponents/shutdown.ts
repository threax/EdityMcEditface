///<amd-module name="edity.local.edit.components.shutdown"/>

"use strict";

import * as navmenu from "edity.editorcore.navmenu";

var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
editMenu.addInjected("ShutdownNavItem", navmenu.ExternalStart + 0, undefined);