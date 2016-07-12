(function ($, h) {
    function FileBrowser(settings) {
        var parentFolders = [];
        var currentFolder = undefined;

        var listFilesUrl = settings.listFilesUrl;

        var directoryList = $(settings.directoryListQuery);
        var directoryComponent = settings.directoryComponent;

        var fileList = $(settings.fileListQuery);
        var fileComponent = settings.fileComponent;

        var upButton = $(settings.upButtonQuery);
        upButton.click(function () {
            currentFolder = parentFolders.pop();
            loadCurrentFolder();
        });

        var loadingLifecycle = new h.lifecycle.ajaxLoad({
            loadingDisplayQuery: '#FileBrowserLoading',
            mainDisplayQuery: '#FileBrowserDisplay',
            loadingFailDisplayQuery: '#FileBrowserLoadFailed'
        });

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
                created.addEventListener('click', function (evt) {
                    self.loadFiles(data);
                    evt.preventDefault();
                });
            });
            h.component.repeat(fileComponent, fileList, data.files);
            if (parentFolders.length === 0) {
                upButton.hide();
            }
            else {
                upButton.show();
            }
        }

        function getFilesFail(data) {
            loadingLifecycle.failed();
        }
    };

    var fileBrowser = new FileBrowser({
        listFilesUrl: "/edity/list",
        fileListQuery: ".filebrowser-file-list",
        fileComponent: "filebrowser-files",
        directoryListQuery: ".filebrowser-directory-list",
        directoryComponent: "filebrowser-directories",
        upButtonQuery: "#FileBrowserUpDirectoryButton"
    });

    $('#AddMediaForm').submit(function () {
        var formData = new FormData(this);
        var filename = $("#FileUploadPicker").val();
        filename = filename.replace(/^.*?([^\\\/]*)$/, '$1');
        h.rest.upload('edity/upload' + fileBrowser.getCurrentDirectory() + '/' + filename, formData,
        function (data) {
            fileBrowser.refresh();
        },
        function (data) {
            alert("File Upload Failed");
        });
        return false;
    });

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "MediaNavItem",
        created: function (button) {
            button.addEventListener('click', function () {
                fileBrowser.loadFiles("/images");
            });
        }
    });
})(jQuery, htmlrest);