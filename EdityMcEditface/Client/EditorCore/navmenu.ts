"use strict";

import * as controller from 'hr.controller';
import { ActionEventDispatcher } from 'hr.eventdispatcher';

interface INavMenuItem {
    name: string;
    created: any;
}

var navMenus = {};

function NavMenu() {
    var menuItems = [];

    var itemAdded = new ActionEventDispatcher<INavMenuItem>();
    this.itemAdded = itemAdded.modifier;

    function add(name, controllerConstructor: any, context?:any) {
        if (controllerConstructor !== undefined) {
            controllerConstructor = controller.createOnCallback(controllerConstructor, context);
        }
        var item = {
            name: name,
            created: controllerConstructor
        };
        menuItems.push(item);
        itemAdded.fire(item);
    }
    this.add = add;

    function getItems() {
        return menuItems;
    }
    this.getItems = getItems;
}

export function getNavMenu(name) {
    var menu = navMenus[name];
    if (menu === undefined) {
        navMenus[name] = menu = new NavMenu();
    }
    return menu;
}