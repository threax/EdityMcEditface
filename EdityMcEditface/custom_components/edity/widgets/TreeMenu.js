"use strict";

jsns.define("edity.widgets.treemenu.editor", [
    "htmlrest.controller",
], function (exports, module, controller) {

    var editTreeMenuItem = null;

    function EditTreeMenuItemController(bindings) {
        editTreeMenuItem = this;

        var dialog = bindings.getToggle("dialog");
        var model = bindings.getModel("properties");
        var currentMenuItem = null;
        var currentEditingCompleteCallback = null;

        function edit(menuItem, editingCompleteCallback) {
            dialog.on();
            model.setData(menuItem);
            currentMenuItem = menuItem;
            currentEditingCompleteCallback = editingCompleteCallback;
        }
        this.edit = edit;

        function updateMenuItem(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            dialog.off();

            var data = model.getData();
            currentMenuItem.name = data.name;

            currentEditingCompleteCallback(currentMenuItem);
            currentMenuItem = null;
            currentEditingCompleteCallback = null;
        }
        this.updateMenuItem = updateMenuItem;
    }

    function createControllers() {
        if (editTreeMenuItem === null) {
            controller.create("editTreeMenuItem", EditTreeMenuItemController);
        }
    }

    function applyChanges(menuData, updateCb) {
        updateCb();
    }

    function moveUp(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        var parent = itemData.parent;
        var loc = parent.folders.indexOf(itemData);
        if (loc > 0) {
            var swap = parent.folders[loc - 1];
            parent.folders[loc - 1] = itemData;
            parent.folders[loc] = swap;
            applyChanges(menuData, updateCb);
        }
    }

    function moveDown(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        var parent = itemData.parent;
        var loc = parent.folders.indexOf(itemData);
        if (loc !== -1 && loc + 1 < parent.folders.length) {
            var swap = parent.folders[loc + 1];
            parent.folders[loc + 1] = itemData;
            parent.folders[loc] = swap;
            applyChanges(menuData, updateCb);
        }
    }

    function addItem(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        alert('addItem');
        applyChanges(menuData, updateCb);
    }

    function editItem(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        editTreeMenuItem.edit(itemData, function(){
            applyChanges(menuData, updateCb);
        });
    }

    function deleteItem(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        //Needs a confirm
        var parent = itemData.parent;
        var loc = parent.folders.indexOf(itemData);
        if (loc !== -1) {
            parent.folders.splice(loc, 1);
            applyChanges(menuData, updateCb);
        }
    }

    function fireItemAdded(menuData, listener, itemData, updateCb) {
        createControllers();

        listener.moveUp = function (evt) {
            moveUp(evt, menuData, itemData, updateCb);
        }

        listener.moveDown = function (evt) {
            moveDown(evt, menuData, itemData, updateCb);
        }

        listener.addItem = function (evt) {
            addItem(evt, menuData, itemData, updateCb);
        }

        listener.editItem = function (evt) {
            editItem(evt, menuData, itemData, updateCb);
        }

        listener.deleteItem = function (evt) {
            deleteItem(evt, menuData, itemData, updateCb);
        }
    }

    exports.fireItemAdded = fireItemAdded;
});

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.components",
    "htmlrest.bindingcollection",
    "htmlrest.domquery",
    "htmlrest.controller",
    "edity.widgets.treemenu.editor"
], function (exports, module, storage, rest, component, BindingCollection, domQuery, controller, treeEditor) {

    controller.create("treeMenu", TreeMenuController);

    /**
     * This function builds a controller that handles a tree menu on the page.
     * @param {type} bindings
     */
    function TreeMenuController(bindings) {
        var rootModel = bindings.getModel('children');
        var config = bindings.getConfig();
        var editMode = config["treemenu-editmode"] === 'true';
        var version = config["treemenu-version"];

        var ajaxurl = rootModel.getSrc();
        var getNextId = (function () {
            var i = -1;
            return function() {
                return ++i;
            }
        })();

        var menuStorageId = 'treemenu-cache-' + ajaxurl;
        var sessionData = storage.getSessionJson(menuStorageId, null);
        var menuCache = null;
        var menuData = null;
        var createdItems = {};
        window.onbeforeunload = function (e) {
            if (editMode) {
                removeParents(menuData);
            }
            storage.storeJsonInSession(menuStorageId,{
                cache: menuCache,
                data: menuData,
                version: version
            });
        };

        if (sessionData === null || sessionData.version !== version) {
            //No data, get it
            menuCache = {};
            rest.get(ajaxurl, function (data) {
                initialSetup(data);
            });
        }
        else {
            //Use what we had
            menuCache = sessionData.cache;
            initialSetup(sessionData.data);
        }

        function initialSetup(data) {
            menuData = data;
            if (menuData !== null) {
                if (data['menuItemId'] === undefined) {
                    createIds(data);
                }

                if (editMode) {
                    findParents(data, null);
                }

                var menuCacheInfo = getMenuCacheInfo(data.menuItemId);
                buildMenu(bindings, menuCacheInfo, menuData, false);
            }
        }

        function rebuildMenu() {
            createdItems = {};
            var childModel = bindings.getModel('children');
            childModel.setData([]);
            var menuCacheInfo = getMenuCacheInfo(menuData.menuItemId);
            buildMenu(bindings, menuCacheInfo, menuData, false);
        }

        function createIds(data) {
            data.menuItemId = getNextId();
            var folders = data.folders;
            for (var i = 0; i < folders.length; ++i) {
                //Recursion, I don't care, how nested is your menu that you run out of stack space here? Can a user really use that?
                createIds(folders[i]);
            }
        }

        function findParents(data, parent) {
            data.parent = parent;
            var folders = data.folders;
            for (var i = 0; i < folders.length; ++i) {
                //Recursion, I don't care, how nested is your menu that you run out of stack space here? Can a user really use that?
                findParents(folders[i], data);
            }
        }

        function removeParents(data) {
            delete data.parent;
            var folders = data.folders;
            for (var i = 0; i < folders.length; ++i) {
                //Recursion, I don't care, how nested is your menu that you run out of stack space here? Can a user really use that?
                removeParents(folders[i]);
            }
        }

        function getMenuCacheInfo(parentCategoryId) {
            if (!menuCache.hasOwnProperty(parentCategoryId)) {
                menuCache[parentCategoryId] = {
                    expanded: false,
                    id: parentCategoryId
                };
            }
            return menuCache[parentCategoryId];
        }

        function buildMenu(parentBindings, menuCacheInfo, folder, autoHide) {
            if (autoHide === undefined) {
                autoHide = true;
            }

            if (!createdItems[menuCacheInfo.id]) {
                var parentModel = parentBindings.getModel('children');
                var list = null;
                parentModel.setData({}, function (created) {
                    list = created;
                });
                createdItems[menuCacheInfo.id] = true;

                var foldersModel = list.getModel('folders');
                var linksModel = list.getModel('links');

                foldersModel.setData(folder.folders, function (folderComponent, data) {
                    var id = data.menuItemId;
                    var menuCacheInfo = getMenuCacheInfo(id);

                    var listener = {
                        toggleMenuItem: function (evt) {
                            evt.preventDefault();

                            buildMenu(folderComponent, menuCacheInfo, data);
                            toggleMenu(menuCacheInfo, folderComponent.getToggle('children'));
                        }
                    };
                    if (editMode) {
                        treeEditor.fireItemAdded(menuData, listener, data, rebuildMenu);
                    }
                    folderComponent.setListener(listener);

                    if (menuCacheInfo.expanded) {
                        buildMenu(folderComponent, menuCacheInfo, data, autoHide);
                    }
                });

                linksModel.setData(folder.pages);
            }
        }

        function toggleMenu(menuCacheInfo, toggle, transitionTime) {
            if (transitionTime === undefined) {
                transitionTime = 200;
            }

            if (menuCacheInfo.expanded) {
                menuCacheInfo.expanded = false;
                toggle.off();
            }
            else {
                menuCacheInfo.expanded = true;
                toggle.on();
            }
        }
    }
});