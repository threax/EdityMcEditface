"use strict";

import * as controller from "hr.controller";
import * as toggles from "hr.toggles";
import * as http from "hr.http";
import * as editorSync from "edity.widgets.TreeMenuEditorSync";
import * as saveService from "clientlibs.SaveService";
import * as EdityClient from 'clientlibs.EdityClient';

var treeMenuEditors = {};
var uploadClient: EdityClient.UploadClient;

function TreeMenuEditor(menuData, updateCb, saveUrl) {
    var hasChanges = false;

    function save() {
        if (hasChanges) {
            hasChanges = false;
            var blob = new Blob([JSON.stringify(menuData, menuJsonSerializeReplacer, 4)], { type: "application/json" });
            return uploadClient.upload(saveUrl, { data: blob, fileName: saveUrl }, null)
                //return http.upload('/edity/upload' + saveUrl, blob)
                .catch(function (data) {
                    hasChanges = true;
                });
        }
    }
    saveService.saveEvent.add(this, save);

    function update() {
        hasChanges = true;
        updateCb();
        saveService.requestSave();
    }
    this.update = update;

    function getMenuData() {
        return menuData;
    }
    this.getMenuData = getMenuData;
}

var editTreeMenuItem = null;
var deleteTreeMenuItem = null;
var addTreeMenuItem = null;
var chooseMenuItem = null;

function deleteMenuItem(menuItem): string | boolean {
    var parent = menuItem.parent;
    var loc = parent.children.indexOf(menuItem);
    if (loc !== -1) {
        menuItem.parent = null;
        parent.children.splice(loc, 1);
        return "folder";
    }
    return false;
}

function EditTreeMenuItemController(bindings) {
    editTreeMenuItem = this;

    var dialog = bindings.getToggle("dialog");
    var model = bindings.getModel("properties");
    var currentMenuItem = null;
    var currentEditingCompleteCallback = null;
    var linkToggle = bindings.getToggle('link');
    var makingLink = false;

    function edit(menuItem, editingCompleteCallback) {
        dialog.on();
        model.setData(menuItem);
        currentMenuItem = menuItem;
        currentEditingCompleteCallback = editingCompleteCallback;
        makingLink = menuItem.hasOwnProperty('link');
        if (makingLink) {
            linkToggle.on();
        }
        else {
            linkToggle.off();
        }
    }
    this.edit = edit;

    function updateMenuItem(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        dialog.off();

        var data = model.getData();
        currentMenuItem.name = data.name;
        if (makingLink) {
            currentMenuItem.link = data.link;
        }

        currentEditingCompleteCallback(currentMenuItem);
        currentMenuItem = null;
        currentEditingCompleteCallback = null;
    }
    this.updateMenuItem = updateMenuItem;
}

function DeleteTreeMenuItemController(bindings) {
    deleteTreeMenuItem = this;

    var dialog = bindings.getToggle('dialog');
    var model = bindings.getModel('info');
    var currentMenuItem = null;
    var currentCallback = null;

    function confirm(menuItem, deleteCallback) {
        currentMenuItem = menuItem;
        currentCallback = deleteCallback;
        model.setData(menuItem);
        dialog.on();
    }
    this.confirm = confirm;

    function deleteItem(evt) {
        evt.preventDefault();
        evt.stopPropagation();

        if (deleteMenuItem(currentMenuItem)) {
            currentCallback();
        }

        currentMenuItem = null;
        currentCallback = null;
        dialog.off();
    }
    this.deleteItem = deleteItem;
}

function AddTreeMenuItemController(bindings) {
    addTreeMenuItem = this;

    var currentParent = null;
    var currentCallback = null;

    var questionModel = bindings.getModel('question');
    var createFolderModel = bindings.getModel('createFolder');
    var createLinkModel = bindings.getModel('createLink');
    var linkAutoTypeModel = bindings.getModel('linkAutoType');

    var dialog = bindings.getToggle('dialog');

    var questionToggle = bindings.getToggle('question');
    var createFolderToggle = bindings.getToggle('createFolder');
    var createLinkToggle = bindings.getToggle('createLink');
    var toggleGroup = new toggles.Group(questionToggle, createFolderToggle, createLinkToggle);
    var autoTypeUrl = true;

    toggleGroup.activate(questionToggle);

    function createNewItem(parent, createdCallback) {
        currentParent = parent;
        currentCallback = createdCallback;
        questionModel.setData(parent);
        toggleGroup.activate(questionToggle);
        dialog.on();
    }
    this.createNewItem = createNewItem;

    function startFolderCreation(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        createFolderModel.clear();

        toggleGroup.activate(createFolderToggle);
    }
    this.startFolderCreation = startFolderCreation;

    function createFolder(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        var folderData = createFolderModel.getData();
        var newItem = {
            name: folderData.name,
            children: [],
            parent: currentParent
        };
        currentParent.children.push(newItem);
        finishAdd(newItem);
    }
    this.createFolder = createFolder;

    function startLinkCreation(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        createLinkModel.clear();
        linkAutoTypeModel.clear();
        autoTypeUrl = true;

        toggleGroup.activate(createLinkToggle);
    }
    this.startLinkCreation = startLinkCreation;

    function createLink(evt) {
        evt.preventDefault();
        evt.stopPropagation();

        var linkData = createLinkModel.getData();
        var newItem = {
            name: linkData.name,
            link: linkData.link,
            parent: currentParent
        };
        currentParent.children.push(newItem);
        finishAdd(newItem);
    }
    this.createLink = createLink;

    function finishAdd(newItem) {
        dialog.off();
        currentCallback(newItem);
        currentCallback = null;
        currentParent = null;
    }

    function replaceUrl(x) {
        switch (x) {
            case ' ':
                return '-';
            default:
                return '';
        }
    }

    function nameChanged(evt) {
        if (autoTypeUrl) {
            var data = createLinkModel.getData();
            var urlName = encodeURI(data.name.replace(/\s|[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, replaceUrl)).toLowerCase();
            linkAutoTypeModel.setData({
                link: '/' + urlName
            });
        }
    }
    this.nameChanged = nameChanged;

    function cancelAutoType() {
        autoTypeUrl = false;
    }
    this.cancelAutoType = cancelAutoType;
}

function menuJsonSerializeReplacer(key, value) {
    if (key === 'parent' || key === 'menuItemId') {
        return undefined;
    }
    return value;
}

function RootNodeControls(bindings, context) {
    function addItem(evt) {
        evt.preventDefault();
        evt.stopPropagation();
        addTreeMenuItem.createNewItem(context.getMenuData(), context.update);
    }
    this.addItem = addItem;
}

function MenuItemChoiceController(bindings, context, data) {
    var row = data;
    var callback = context;

    function itemChosen(evt) {
        evt.preventDefault();
        callback(row);
    }

    this.itemChosen = itemChosen;
}

function ChooseMenuItemController(bindings, context) {
    chooseMenuItem = this;

    var dialog = bindings.getToggle("dialog");
    var promptModel = bindings.getModel("prompt");
    var chooser = bindings.getModel("chooser");
    var currentChosenCb;

    function chosen(item) {
        currentChosenCb(item);
        dialog.off();
    }

    function chooseItem(prompt, items, chosenCb) {
        promptModel.setData(prompt);
        dialog.on();
        currentChosenCb = chosenCb;
        chooser.setData(items, controller.createOnCallback(<any>MenuItemChoiceController, chosen));
    }
    this.chooseItem = chooseItem;
}

function createControllers() {
    if (editTreeMenuItem === null) {
        controller.create("editTreeMenuItem", <any>EditTreeMenuItemController);
        controller.create("deleteTreeMenuItem", <any>DeleteTreeMenuItemController);
        controller.create("createTreeMenuItem", <any>AddTreeMenuItemController);
        controller.create("chooseTreeMenuItem", <any>ChooseMenuItemController);
    }
}

function moveUp(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();
    var parent = itemData.parent;
    var loc = parent.children.indexOf(itemData);
    if (loc !== -1) {
        if (loc > 0) {
            var swap = parent.children[loc - 1];
            parent.children[loc - 1] = itemData;
            parent.children[loc] = swap;
            updateCb();
        }
    }
}

function moveDown(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();
    var parent = itemData.parent;
    var loc = parent.children.indexOf(itemData);
    if (loc !== -1) {
        if (loc + 1 < parent.children.length) {
            var swap = parent.children[loc + 1];
            parent.children[loc + 1] = itemData;
            parent.children[loc] = swap;
            updateCb();
        }
    }
}

function addItem(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();
    addTreeMenuItem.createNewItem(itemData, function () {
        updateCb();
    });
}

function editItem(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();
    editTreeMenuItem.edit(itemData, function () {
        updateCb();
    });
}

function deleteItem(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();
    deleteTreeMenuItem.confirm(itemData, function () {
        updateCb();
    });
}

function moveToParent(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();
    var parent = itemData.parent;
    var superParent = parent.parent;
    if (superParent) {
        var loc = parent.children.indexOf(itemData);
        if (loc !== -1) {
            var swap = parent.children[loc];
            parent.children.splice(loc, 1);
            superParent.children.push(swap);
            swap.parent = superParent;
            updateCb();
        }
    }
}

function itemIter(items, skipItem) {
    var i = 0;
    return function () {
        var item = null;
        if (i < items.length) {
            item = items[i++];
            while (item != null && (item === skipItem || !editorSync.isFolder(item))) {
                if (i < items.length) {
                    item = items[i++];
                }
                else {
                    item = null;
                }
            }
        }
        return item;
    }
}

function moveToChild(evt, menuData, itemData, updateCb) {
    evt.preventDefault();
    evt.stopPropagation();

    chooseMenuItem.chooseItem("Nest " + itemData.name + " under...", itemIter(itemData.parent.children, itemData), function (selectedItem) {
        var result = deleteMenuItem(itemData);
        if (result) {
            selectedItem.children.push(itemData);
            itemData.parent = selectedItem;
            updateCb();
        }
    });
}

function fireItemAdded(saveUrl, itemData, bindListenerCb) {
    createControllers();
    var treeEditor = treeMenuEditors[saveUrl];

    if (!treeEditor) {
        throw new Error('Cannot find tree menu editor for "' + saveUrl + '" did you create the root controller?');
    }

    bindListenerCb({
        moveUp: function (evt) {
            moveUp(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        },

        moveDown: function (evt) {
            moveDown(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        },

        addItem: function (evt) {
            addItem(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        },

        editItem: function (evt) {
            editItem(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        },

        deleteItem: function (evt) {
            deleteItem(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        },

        moveToParent: function (evt) {
            moveToParent(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        },

        moveToChild: function (evt) {
            moveToChild(evt, treeEditor.getMenuData(), itemData, treeEditor.update);
        }
    });
}

function createRootNodeControls(controllerElementName, menuData, updateCb, saveUrl, parentBindings) {
    var treeMenuEditor = treeMenuEditors[saveUrl];
    if (treeMenuEditor === undefined) {
        treeMenuEditor = new TreeMenuEditor(menuData, updateCb, saveUrl);
        treeMenuEditors[saveUrl] = treeMenuEditor;
    }
    controller.create(controllerElementName, <any>RootNodeControls, treeMenuEditor, parentBindings);
}

editorSync.setEditorListener({
    itemAdded: fireItemAdded,
    createRootNodeControls: createRootNodeControls
});