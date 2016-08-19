"use strict";
jsns.run([
    "hr.http",
    "hr.storage",
    "hr.bindingcollection",
    "hr.toggles",
    "hr.iterable",
    "hr.controller",
    "hr.widgets.navmenu"
],
function (exports, module, http, storage, BindingCollection, toggles, Iterable, controller, navmenu) {

    function getFileName(path) {
        return path.replace(/^.*?([^\\\/]*)$/, '$1');
    }

    /**
     * Create a file browser
     * @param {BindingCollection} bindings
     * @param {FileBrowserSettings} [settings]
     */
    function FileBrowser(bindings) {
        var parentFolders = [];
        var currentFolder = undefined;

        var directoryModel = bindings.getModel('directories');
        var fileModel = bindings.getModel('files');
        var listFilesUrl = directoryModel.getSrc();

        var upDir = bindings.getToggle('upDir');

        bindings.setListener({
            upDirectory: function () {
                currentFolder = parentFolders.pop();
                loadCurrentFolder();
            }
        });

        var load = bindings.getToggle('load');
        var main = bindings.getToggle('main');
        var fail = bindings.getToggle('fail');
        var toggleGroup = new toggles.Group(load, main, fail);

        var self = this;

        this.loadFiles = function (path) {
            if (currentFolder !== undefined) {
                parentFolders.push(currentFolder);
            }
            currentFolder = path;
            loadCurrentFolder();
        }

        this.getCurrentDirectory = function () {
            return currentFolder;
        }

        this.refresh = function () {
            loadCurrentFolder();
        }

        function loadCurrentFolder() {
            toggleGroup.show(load);
            http.get(listFilesUrl + currentFolder)
            .then(getFilesSuccess)
            .catch(getFilesFail);
        }

        function getFilesSuccess(data) {
            toggleGroup.show(main);

            var iter = new Iterable(data.directories)
                       .select(function (i) {
                           return { name: getFileName(i), link: i };
                       });

            directoryModel.setData(iter,
                function (created, data) {
                    var link = data.link;
                    created.setListener({
                        changeDirectory: function (evt) {
                            evt.preventDefault();
                            self.loadFiles(link);
                        }
                    });
                });

            iter = new Iterable(data.files)
                       .select(function (i) {
                           return { name: getFileName(i), link: i };
                       });

            fileModel.setData(iter);

            if (parentFolders.length === 0) {
                upDir.off();
            }
            else {
                upDir.on();
            }
        }

        function getFilesFail(data) {
            toggleGroup.show(fail);
        }
    };

    function MediaController(bindings) {
        var fileBrowser = new FileBrowser(bindings);
        var uploadModel = bindings.getModel('upload');
        var dialog = bindings.getToggle('dialog');

        function upload(evt) {
            evt.preventDefault();

            var formData = new FormData(this);
            var filename = uploadModel.getData()["file"];
            filename = getFileName(filename);
            http.upload(uploadModel.getSrc() + fileBrowser.getCurrentDirectory() + '/' + filename, formData)
            .then(function (data) {
                fileBrowser.refresh();
            })
            .catch(function (data) {
                alert("File Upload Failed");
            });
        }
        this.upload = upload;

        function NavButtonController(button) {
            function loadMedia() {
                var model = button.getModel('browse');
                fileBrowser.loadFiles(model.getSrc());
                dialog.on();
            }
            this.loadMedia = loadMedia;
        }

        var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
        editMenu.add("MediaNavItem", NavButtonController);
    }

    controller.create("media", MediaController);
});