"use strict";

jsns.define("edity.widgets.treemenu.editorSync", [
    "hr.lateboundeventhandler"
], function (exports, module, LateBoundEvent) {
    var itemAdded = new LateBoundEvent();
    var createRootNodeControls = new LateBoundEvent();

    function fireItemAdded(saveUrl, itemData, bindListenerCb) {
        itemAdded.fire(saveUrl, itemData, bindListenerCb);
    }
    exports.fireItemAdded = fireItemAdded;

    function fireCreateRootNodeControls(controllerElementName, menuData, updateCb, saveUrl, parentBindings) {
        createRootNodeControls.fire(controllerElementName, menuData, updateCb, saveUrl, parentBindings);
    }
    exports.fireCreateRootNodeControls = fireCreateRootNodeControls;

    function setEditorListener(value) {
        //Important order, need to create root nodes first
        createRootNodeControls.modifier.add(value, value.createRootNodeControls);
        itemAdded.modifier.add(value, value.itemAdded);
    }
    exports.setEditorListener = setEditorListener;

    function isFolder(menuItem) {
        return !menuItem.hasOwnProperty("link");
    }
    exports.isFolder = isFolder;
});

jsns.define("edity.widgets.treemenu.controller", [
    "hr.storage",
    "hr.http",
    "hr.controller",
    "edity.widgets.treemenu.editorSync"
], function (exports, module, storage, http, controller, editorSync) {
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
            return function () {
                return ++i;
            }
        })();

        var menuStorageId = 'treemenu-cache-' + ajaxurl;
        var sessionData = storage.getSessionJson(menuStorageId, null);
        var menuCache = null;
        var menuData = null;
        var createdItems = {
        };
        window.onbeforeunload = function (e) {
            if (editMode) {
                removeParents(menuData);
            }
            storage.storeJsonInSession(menuStorageId, {
                cache: menuCache,
                data: menuData,
                version: version
            });
        };

        if (sessionData === null || sessionData.version !== version) {
            //No data, get it
            menuCache = {
            };
            http.get(ajaxurl)
            .then(function (data) {
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
                    editorSync.fireCreateRootNodeControls("treeMenuEditRoot", menuData, rebuildMenu, ajaxurl, bindings); //This isn't really right, will create controllers for all tree menus on the page, need to single out somehow
                }

                var menuCacheInfo = getMenuCacheInfo(data.menuItemId);
                buildMenu(bindings, menuCacheInfo, menuData, false);
            }
        }

        function rebuildMenu() {
            createdItems = {
            };
            var childModel = bindings.getModel('children');
            childModel.setData([]);
            var menuCacheInfo = getMenuCacheInfo(menuData.menuItemId);
            buildMenu(bindings, menuCacheInfo, menuData, false);
        }

        function createIds(data) {
            if (editorSync.isFolder(data)) {
                data.menuItemId = getNextId();
                var children = data.children;
                for (var i = 0; i < children.length; ++i) {
                    //Recursion, I don't care, how nested is your menu that you run out of stack space here? Can a user really use that?
                    createIds(children[i]);
                }
            }
        }

        function findParents(data, parent) {
            data.parent = parent;
            var children = data.children;
            if (children) {
                for (var i = 0; i < children.length; ++i) {
                    //Recursion, I don't care, how nested is your menu that you run out of stack space here? Can a user really use that?
                    findParents(children[i], data);
                }
            }
        }

        function removeParents(data) {
            delete data.parent;
            var children = data.children;
            if (children) {
                for (var i = 0; i < children.length; ++i) {
                    //Recursion, I don't care, how nested is your menu that you run out of stack space here? Can a user really use that?
                    removeParents(children[i]);
                }
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
                parentModel.setData({
                }, function (created) {
                    list = created;
                });
                createdItems[menuCacheInfo.id] = true;

                var childItemsModel = list.getModel('childItems');

                childItemsModel.setData(folder.children, function (folderComponent, data) {
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
                        editorSync.fireItemAdded(ajaxurl, data, function (editListener) { folderComponent.setListener(editListener); });
                    }
                    folderComponent.setListener(listener);

                    if (menuCacheInfo.expanded) {
                        buildMenu(folderComponent, menuCacheInfo, data, autoHide);
                    }
                }, function (row) {
                    if (!editorSync.isFolder(row)) {
                        return "link";
                    }
                });
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

    exports.TreeMenuController = TreeMenuController;
});

jsns.run([
    "hr.controller",
    "edity.widgets.treemenu.controller"
], function (exports, module, controller, treemenu) {
    controller.create("treeMenu", treemenu.TreeMenuController);
});