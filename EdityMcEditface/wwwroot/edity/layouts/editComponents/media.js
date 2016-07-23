"use strict";
jsns.run(function (using) {
    var rest = using("htmlrest.rest");
    var storage = using("htmlrest.storage");
    var BindingCollection = using("htmlrest.bindingcollection").BindingCollection;
    var toggles = using("htmlrest.toggles");
    var iter = using("htmlrest.iter").iter;

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
        var toggleGroup = new toggles.group(load, main, fail);

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
            rest.get(listFilesUrl + currentFolder, getFilesSuccess, getFilesFail);
        }

        function getFilesSuccess(data) {
            toggleGroup.show(main);

            directoryModel.setData(iter(data.directories, function (dir) { return { name: getFileName(dir), link: dir } }),
                function (created, data) {
                    var link = data.link;
                    created.setListener({
                        changeDirectory: function (evt) {
                            evt.preventDefault();
                            self.loadFiles(link);
                        }
                    });
                });

            fileModel.setData(iter(data.files, function (file) { return { name: getFileName(file), link: file } }));

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

    var bindings = new BindingCollection("#mediaModal");

    var fileBrowser = new FileBrowser(bindings);
    var uploadModel = bindings.getModel('upload');

    bindings.setListener({
        upload: function (evt) {
            evt.preventDefault();

            var formData = new FormData(this);
            var filename = uploadModel.getData()["file"];
            filename = getFileName(filename);
            rest.upload(uploadModel.getSrc() + fileBrowser.getCurrentDirectory() + '/' + filename, formData,
            function (data) {
                fileBrowser.refresh();
            },
            function (data) {
                alert("File Upload Failed");
            });
        }
    });

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "MediaNavItem",
        created: function (button) {
            button.setListener({
                loadMedia: function () {
                    var model = button.getModel('browse');
                    fileBrowser.loadFiles(model.getSrc());
                }
            });
        }
    });
});