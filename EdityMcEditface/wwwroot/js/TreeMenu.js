"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.components",
    "htmlrest.bindingcollection",
    "htmlrest.domquery",
    "htmlrest.controller"
], function (exports, module, storage, rest, component, BindingCollection, domQuery, controller) {

    controller.create("treeMenu", TreeMenuController);

    /**
     * This function builds a controller that handles a tree menu on the page.
     * @param {type} bindings
     */
    function TreeMenuController(bindings) {
        var rootModel = bindings.getModel('children');

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
                    folderComponent.setListener({
                        toggleMenuItem: function (evt) {
                            evt.preventDefault();

                            var menuCacheInfo = getMenuCacheInfo(id);
                            buildMenu(folderComponent, menuCacheInfo);
                            toggleMenu(menuCacheInfo, folderComponent.getToggle('children'));
                        }
                    });
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