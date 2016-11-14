"use strict";

import * as LateBoundEvent from "hr.lateboundeventhandler";

var itemAdded = new LateBoundEvent.LateBoundEventHandler();
var createRootNodeControls = new LateBoundEvent.LateBoundEventHandler();

export function fireItemAdded(saveUrl, itemData, bindListenerCb) {
    itemAdded.fire(saveUrl, itemData, bindListenerCb);
}

export function fireCreateRootNodeControls(controllerElementName, menuData, updateCb, saveUrl, parentBindings) {
    createRootNodeControls.fire(controllerElementName, menuData, updateCb, saveUrl, parentBindings);
}

export function setEditorListener(value) {
    //Important order, need to create root nodes first
    createRootNodeControls.modifier.add(value, value.createRootNodeControls);
    itemAdded.modifier.add(value, value.itemAdded);
}

export function isFolder(menuItem) {
    return !menuItem.hasOwnProperty("link");
}