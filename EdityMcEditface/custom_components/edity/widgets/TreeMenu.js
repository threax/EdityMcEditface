"use strict";

jsns.define("edity.widgets.treemenu.editor", [
], function (exports, module) {
    function applyChanges(menuData, updateCb) {
        updateCb(null);
        updateCb(menuData);
    }

    function moveUp(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        alert('moveup');
        applyChanges(menuData, updateCb);
    }

    function moveDown(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        alert('moveDown');
        applyChanges(menuData, updateCb);
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
        alert('editItem');
        applyChanges(menuData, updateCb);
    }

    function deleteItem(evt, menuData, itemData, updateCb) {
        evt.preventDefault();
        evt.stopPropagation();
        alert('deleteItem');
        applyChanges(menuData, updateCb);
    }

    function fireItemAdded(menuData, listener, itemData, updateCb) {

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

        var ajaxurl = rootModel.getSrc();

        var sessionStorageCache = 'treemenu-categorycache-' + ajaxurl;
        var sessionStorageMenu = 'treemenu-sessionstorage-' + ajaxurl;
        var menuCache = storage.getSessionJson(sessionStorageCache, {});
        var menuParentLists = {};
        var menuData = null;
        window.onbeforeunload = function (e) {
            storage.storeJsonInSession(sessionStorageCache, menuCache);
            storage.storeJsonInSession(sessionStorageMenu, menuData);
        };

        initialSetup(storage.getSessionJson(sessionStorageMenu));

        rest.get(ajaxurl, function (data) {
            initialSetup(data);
        });

        function initialSetup(data) {
            //are we the same
            if (menuData !== null && data !== null) {
                if (menuData.instanceId === data.instanceId) {
                    return;
                }
                else {
                    //Clear old menu info, the menu was changed
                    menuCache = {};
                    menu.empty();
                    menuParentLists = {};
                }
            }

            menuData = data;
            if (menuData !== null) {
                var menuCacheInfo = getMenuCacheInfo(0);
                buildMenu(bindings, menuCacheInfo, false);
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

        function buildMenu(parentBindings, menuCacheInfo, autoHide) {
            if (autoHide === undefined) {
                autoHide = true;
            }

            var list = menuParentLists[menuCacheInfo.id];
            if (list === undefined) {
                var parentModel = parentBindings.getModel('children');
                var categories = menuData.entries[menuCacheInfo.id];
                parentModel.setData({}, function (created) {
                    list = created;
                });
                menuParentLists[menuCacheInfo.id] = list;

                var foldersModel = list.getModel('folders');
                var linksModel = list.getModel('links');

                foldersModel.setData(categories.folders, function (folderComponent, data) {
                    var id = data.id;

                    var listener = {
                        toggleMenuItem: function (evt) {
                            evt.preventDefault();

                            var menuCacheInfo = getMenuCacheInfo(id);
                            buildMenu(folderComponent, menuCacheInfo);
                            toggleMenu(menuCacheInfo, folderComponent.getToggle('children'));
                        }
                    };
                    if (editMode) {
                        treeEditor.fireItemAdded(menuData, listener, data, initialSetup);
                    }
                    folderComponent.setListener(listener);

                    var childData = getMenuCacheInfo(data.id);
                    if (childData.expanded) {
                        buildMenu(folderComponent, childData, autoHide);
                    }
                });

                linksModel.setData(categories.pages);
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