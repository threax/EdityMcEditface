(function (h) {
    /**
     * @constructor
     */
    function FileBrowserSettings() {
        var self = this;

        this.listFilesUrl = "/edity/list",
        this.fileComponent = "filebrowser-files",
        this.directoryComponent = "filebrowser-directories",

        this.fileList = "fileList",
        this.directoryList = "directoryList",
        this.upButton = "upDirectory"

        this.mainDisplay = "main";
        this.failDisplay = "fail";
        this.loadDisplay = "load";
        
        this.getFileList = function (bindings) {
            if (htmlrest.isString(self.fileList)) {
                return bindings.first(self.fileList);
            }
            return self.fileList;
        }

        this.getDirectoryList = function (bindings) {
            if (htmlrest.isString(self.directoryList)) {
                return bindings.first(self.directoryList);
            }
            return self.directoryList;
        }

        this.getUpButton = function (bindings) {
            if (htmlrest.isString(self.upButton)) {
                return bindings.first(self.upButton);
            }
            return self.upButton;
        }
    }

    /**
     * Create a file browser
     * @param {htmlrest.component.BindingCollection} bindings
     * @param {htmlrest.component.FileBrowserSettings} [settings]
     */
    function FileBrowser(bindings, settings) {
        if (settings === undefined) {
            settings = new FileBrowserSettings();
        }

        var parentFolders = [];
        var currentFolder = undefined;

        var listFilesUrl = settings.listFilesUrl;

        var directoryList = settings.getDirectoryList(bindings);
        var directoryComponent = settings.directoryComponent;

        var fileList = settings.getFileList(bindings);
        var fileComponent = settings.fileComponent;

        var upButton = settings.getUpButton(bindings);
        upButton.addEventListener('click', function () {
            currentFolder = parentFolders.pop();
            loadCurrentFolder();
        });

        var loadDisplayFailSettings = new h.lifecycle.LoadDisplayFailSettings();
        loadDisplayFailSettings.mainDisplay = settings.mainDisplay;
        loadDisplayFailSettings.failDisplay = settings.failDisplay;
        loadDisplayFailSettings.loadDisplay = settings.loadDisplay;

        var loadingLifecycle = new h.lifecycle.LoadDisplayFail(bindings, loadDisplayFailSettings);

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
            loadingLifecycle.loading();
            h.rest.get(listFilesUrl + currentFolder, getFilesSuccess, getFilesFail);
        }

        function getFilesSuccess(data) {
            loadingLifecycle.succeeded();
            h.component.empty(directoryList);
            h.component.empty(fileList);
            h.component.repeat(directoryComponent, directoryList, data.directories, function (created, data) {
                created.bind({
                    DirectoryButton: {
                        click: function (evt) {
                            self.loadFiles(data);
                            evt.preventDefault();
                        }
                    }
                });
            });
            h.component.repeat(fileComponent, fileList, data.files);
            if (parentFolders.length === 0) {
                upButton.style.display = "none";
            }
            else {
                upButton.style.display = "";
            }
        }

        function getFilesFail(data) {
            loadingLifecycle.failed();
        }
    };

    var bindings = new htmlrest.component.BindingCollection("#mediaModal");

    var fileBrowser = new FileBrowser(bindings);
    var fileUploadPicker = bindings.first("FileUploadPicker");

    bindings.bind({
        AddMediaForm: {
            submit: function (evt) {
                evt.preventDefault();

                var formData = new FormData(this);
                var filename = fileUploadPicker.value;
                filename = filename.replace(/^.*?([^\\\/]*)$/, '$1');
                h.rest.upload('edity/upload' + fileBrowser.getCurrentDirectory() + '/' + filename, formData,
                function (data) {
                    fileBrowser.refresh();
                },
                function (data) {
                    alert("File Upload Failed");
                });
            }
        }
    });

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "MediaNavItem",
        created: function (button) {
            button.bind({
                MediaModalButton: {
                    click: function(){
                        fileBrowser.loadFiles("/images");
                    }
                }
            });
        }
    });
})(htmlrest);