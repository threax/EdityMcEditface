"use strict";

import * as uploader from "clientlibs.uploader";
import * as storage from "hr.storage";
import * as BindingCollection from "hr.bindingcollection";
import * as toggles from "hr.toggles";
import * as Iterable from "hr.iterable";
import * as controller from "hr.controller";
import * as navmenu from "hr.widgets.navmenu";
import * as EdityClient from 'clientlibs.EdityClient';
import * as PageStart from 'clientlibs.PageStart';

function getFileName(path) {
    return path.replace(/^.*?([^\\\/]*)$/, '$1');
}

/**
 * Create a file browser
 */
function FileBrowser(bindings, uploadClient: EdityClient.UploadClient) {
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
        toggleGroup.activate(load);
        uploadClient.listFiles(currentFolder, null)
            .then(getFilesSuccess)
            .catch(getFilesFail);
    }

    function getFilesSuccess(data) {
        toggleGroup.activate(main);

        var iter = new Iterable.Iterable(data.directories)
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

        iter = new Iterable.Iterable(data.files)
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
        toggleGroup.activate(fail);
    }
};

class NavButtonController {
    private bindings: controller.BindingCollection;
    private mediaControllerInstance: MediaController;

    constructor(bindings, mediaControllerInstance: MediaController) {
        this.bindings = bindings;
        this.mediaControllerInstance = mediaControllerInstance;
    }

    loadMedia() {
        this.mediaControllerInstance.loadMedia(this.bindings.getModel("browse").getSrc());
    }
}

class MediaController {
    private fileBrowser;
    private uploadModel;
    private dialog;

    constructor(bindings: controller.BindingCollection, context: PageStart.PageStart) {
        this.fileBrowser = new FileBrowser(bindings, new EdityClient.UploadClient(context.BaseUrl, context.Fetcher));
        this.uploadModel = bindings.getModel('upload');
        this.dialog = bindings.getToggle('dialog');
    }

    upload(evt) {
        evt.preventDefault();

        var formData = new FormData();
        var filename = this.uploadModel.getData()["file"];
        filename = getFileName(filename);
        uploader.upload(this.uploadModel.getSrc() + this.fileBrowser.getCurrentDirectory() + '/' + filename, formData)
            .then(function (data) {
                this.fileBrowser.refresh();
            })
            .catch(function (data) {
                alert("File Upload Failed");
            });
    }

    loadMedia(source) {
        this.fileBrowser.loadFiles(source);
        this.dialog.on();
    }
}

PageStart.init().then((config) => {
    var mediaControllerInstance = controller.create<MediaController, PageStart.PageStart, void>("media", MediaController, config)[0];

    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("MediaNavItem", NavButtonController, mediaControllerInstance);
});