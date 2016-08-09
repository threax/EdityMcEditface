"use strict";

jsns.run([
    "htmlrest.controller",
    "htmlrest.toggles",
    "htmlrest.rest",
    "edity.widgets.treemenu.editorSync"
], function (exports, module, controller, toggles, rest, editorSync) {

    var editTreeMenuItem = null;
    var deleteTreeMenuItem = null;
    var addTreeMenuItem = null;

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

            var parent = currentMenuItem.parent;
            var loc = parent.folders.indexOf(currentMenuItem);
            if (loc !== -1) {
                parent.folders.splice(loc, 1);
                currentCallback();
            }
            else {
                loc = parent.links.indexOf(currentMenuItem);
                if (loc !== -1) {
                    parent.links.splice(loc, 1);
                    currentCallback();
                }
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

        toggleGroup.show(questionToggle);

        function createNewItem(parent, createdCallback) {
            currentParent = parent;
            currentCallback = createdCallback;
            questionModel.setData(parent);
            toggleGroup.show(questionToggle);
            dialog.on();
        }
        this.createNewItem = createNewItem;

        function startFolderCreation(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            createFolderModel.clear();

            toggleGroup.show(createFolderToggle);
        }
        this.startFolderCreation = startFolderCreation;

        function createFolder(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            var folderData = createFolderModel.getData();
            var newItem = {
                name: folderData.name,
                folders: [],
                links: [],
                parent: currentParent
            };
            currentParent.folders.push(newItem);
            finishAdd(newItem);
        }
        this.createFolder = createFolder;

        function startLinkCreation(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            createLinkModel.clear();
            linkAutoTypeModel.clear();
            autoTypeUrl = true;

            toggleGroup.show(createLinkToggle);
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
            currentParent.links.push(newItem);
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
                var urlName = encodeURI(data.name.replace(/\s|[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, replaceUrl));
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
        var menuData = context.menuData;
        var updateCb = context.updateCb;
        var saveUrl = context.saveUrl;
        var uploadUrl = bindings.getModel('treeMenuEditRoot').getSrc();
        var loading = bindings.getToggle('loading');
        loading.off();

        function save(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            loading.on();
            var blob = new Blob([JSON.stringify(menuData, menuJsonSerializeReplacer, 4)], { type: "application/json" });
            rest.upload(uploadUrl + saveUrl, blob, function () {
                loading.off();
            }, function () {
                alert('Error saving menu, please try again later.')
                loading.off();
            });
        }
        this.save = save;

        function addItem(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            addTreeMenuItem.createNewItem(menuData, function () {
                updateCb();
            });
        }
        this.addItem = addItem;
    }

    function createControllers() {
        if (editTreeMenuItem === null) {
            controller.create("editTreeMenuItem", EditTreeMenuItemController);
            controller.create("deleteTreeMenuItem", DeleteTreeMenuItemController);
            controller.create("createTreeMenuItem", AddTreeMenuItemController);
        }
    }

    function moveUp(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        var parent = itemData.parent;
        var loc = parent.folders.indexOf(itemData);
        if (loc !== -1) {
            if (loc > 0) {
                var swap = parent.folders[loc - 1];
                parent.folders[loc - 1] = itemData;
                parent.folders[loc] = swap;
                updateCb();
            }
        }
        else {
            loc = parent.links.indexOf(itemData);
            if (loc > 0) {
                var swap = parent.links[loc - 1];
                parent.links[loc - 1] = itemData;
                parent.links[loc] = swap;
                updateCb();
            }
        }
    }

    function moveDown(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        var parent = itemData.parent;
        var loc = parent.folders.indexOf(itemData);
        if (loc !== -1) {
            if (loc + 1 < parent.folders.length) {
                var swap = parent.folders[loc + 1];
                parent.folders[loc + 1] = itemData;
                parent.folders[loc] = swap;
                updateCb();
            }
        }
        else {
            loc = parent.links.indexOf(itemData);
            if (loc !== -1 && loc + 1 < parent.links.length) {
                var swap = parent.links[loc + 1];
                parent.links[loc + 1] = itemData;
                parent.links[loc] = swap;
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
            var loc = parent.folders.indexOf(itemData);
            if (loc !== -1) {
                var swap = parent.folders[loc];
                parent.folders.splice(loc, 1);
                superParent.folders.push(swap);
                swap.parent = superParent;
                updateCb();
            }
            else {
                loc = parent.links.indexOf(itemData);
                if (loc !== -1) {
                    var swap = parent.links[loc];
                    parent.links.splice(loc, 1);
                    superParent.links.push(swap);
                    swap.parent = superParent;
                    updateCb();
                }
            }
        }
    }

    function moveToChild(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
    }

    function fireItemAdded(menuData, bindListenerCb, itemData, updateCb) {
        createControllers(menuData);

        bindListenerCb({
            moveUp: function (evt) {
                moveUp(evt, menuData, itemData, updateCb);
            },

            moveDown: function (evt) {
                moveDown(evt, menuData, itemData, updateCb);
            },

            addItem: function (evt) {
                addItem(evt, menuData, itemData, updateCb);
            },

            editItem: function (evt) {
                editItem(evt, menuData, itemData, updateCb);
            },

            deleteItem: function (evt) {
                deleteItem(evt, menuData, itemData, updateCb);
            },

            moveToParent: function (evt) {
                moveToParent(evt, menuData, itemData, updateCb);
            },

            moveToChild: function (evt) {
                moveToChild(evt, menuData, itemData, updateCb);
            }
        });
    }

    function createRootNodeControls(controllerElementName, menuData, updateCb, saveUrl) {
        controller.create(controllerElementName, RootNodeControls, {
            menuData: menuData,
            updateCb: updateCb,
            saveUrl: saveUrl
        });
    }

    editorSync.setEditorListener({
        itemAdded: fireItemAdded,
        createRootNodeControls: createRootNodeControls
    });
});