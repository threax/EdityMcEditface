(function ($, h) {

    var loading = $(".load-linebar");

    $('#CommitButton').click(function () {
        h.rest.get('edity/Git/UncommittedChanges', function (data) {
            var parent = $('.git-uncommitted-changes-list');
            h.component.empty(parent);
            h.component.repeat("git-uncommitted-change", parent, data);
        });
    });

    $(".git-commit-form").submit(function (event) {
        var data = h.form.serialize($(this));
        h.rest.post('edity/Git/Commit', data,
        function (data) {
            $('#commitModal').modal('hide');
        }, function (data) {
            alert('Error Committing');
        });
        return false;
    });

    //Compiler
    function CompilerMessages() {
        var startMessage = $('[data-compile-message-startup]');
        var successMessage = $('[data-compile-message-success]');
        var failMessage = $('[data-compile-message-fail]');
        var compilingMessage = $('[data-compile-compiling]');
        var self = this;

        this.hideAll = function () {
            startMessage.hide();
            successMessage.hide();
            failMessage.hide();
            compilingMessage.hide();
        }

        this.starting = function () {
            self.hideAll();
            startMessage.show();
        }

        this.compiling = function () {
            self.hideAll();
            compilingMessage.show();
        }

        this.succeeded = function () {
            self.hideAll();
            successMessage.show();
        }

        this.failed = function () {
            self.hideAll();
            failMessage.show();
        }
    };

    var compilerOutputMessage = new CompilerMessages();

    $('#CompileModalButton').click(function () {
        compilerOutputMessage.starting();
    });

    var compileButton = $('#CompileButton');

    compileButton.click(function () {
        compileButton.prop('disabled', true);
        compilerOutputMessage.compiling();
        h.rest.post('/edity/Compile', {},
        function () {
            compileButton.prop('disabled', false);
            compilerOutputMessage.succeeded();
        }, function () {
            compileButton.prop('disabled', false);
            compilerOutputMessage.failed();
        });
        return false;
    });
    //End Compiler

    var settingsLifecycle = new h.form.ajaxLifecycle({
        formQuery: '#SettingsForm',
        loadingDisplayQuery: '#SettingsLoading',
        mainDisplayQuery: '#SettingsForm',
        populateFailDisplayQuery: '#SettingsLoadFailed'
    });

    $('#SettingsButton').click(function () {
        settingsLifecycle.populateData();
    });

    $('#LoadSettingsAgainButton').click(function () {
        settingsLifecycle.populateData();
    });

    //--------------- File Browser ----------------

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

        this.getCurrentDirectory = function(){
            return currentFolder;
        }

        this.refresh = function(){
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
                created.click(function () {
                    self.loadFiles(data);
                    return false;
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
    })

    $('#MediaModalButton').click(function () {
        fileBrowser.loadFiles("/images");
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
})(jQuery, htmlrest);