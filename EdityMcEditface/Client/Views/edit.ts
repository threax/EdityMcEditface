"use strict";

import * as component from "hr.components";
import * as domQuery from "hr.domquery";
import * as navmenu from "edity.editorcore.navmenu";

import * as controller from 'hr.controller';

var navMenu = domQuery.first('[data-editor-navmenu]');

function itemAdded(item) {
    component.single(item.name, navMenu, null, item.created);
}

var menu = navmenu.getNavMenu("edit-nav-menu-items");
menu.getItems().forEach(itemAdded);
menu.itemAdded.add(itemAdded);

class ToolsViewController {
    public static Builder() {
        return new controller.ControllerBuilder<ToolsViewController, void, void>(ToolsViewController);
    }

    private mainToggle: controller.OnOffToggle;

    constructor(bindings: controller.BindingCollection) {
        this.mainToggle = bindings.getToggle("main");
    }

    public toggleTools(evt: Event) {
        evt.preventDefault();
        this.mainToggle.toggle();
    }
}


var builder = ToolsViewController.Builder();
builder.create("edity-tools");