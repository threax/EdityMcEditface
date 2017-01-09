"use strict";

import * as controller from 'hr.controller';
import { ActionEventDispatcher } from 'hr.eventdispatcher';

interface INavMenuItem {
    name: string;
    created: any;
}

var navMenus = {};

class NavMenu {
    private menuItems: INavMenuItem[] = [];
    private itemAddedEvent = new ActionEventDispatcher<INavMenuItem>();

    constructor(){
    
    }

    public get itemAdded() {
        return this.itemAddedEvent.modifier;
    }

    public add(name, controllerConstructor?: any, context?:any) {
        if (controllerConstructor !== undefined) {
            controllerConstructor = controller.createOnCallback(controllerConstructor, context);
        }
        var item = {
            name: name,
            created: controllerConstructor
        };
        this.menuItems.push(item);
        this.itemAddedEvent.fire(item);
    }

    public getItems() {
        return this.menuItems;
    }
}

export function getNavMenu(name: string): NavMenu {
    var menu = navMenus[name];
    if (menu === undefined) {
        navMenus[name] = menu = new NavMenu();
    }
    return menu;
}