"use strict";
import * as clientBuild from './clientbuild';

var copy = require('threax-npm-tk/copy');

var filesDir = __dirname + "/..";

export function build(outDir, iconOutPath, moduleDir): Promise<any> {
    var promises = [];

    var libDir = outDir + '/edity/lib';

    promises.push(clientBuild.build(outDir, iconOutPath, moduleDir));
    promises.push(buildCkEditor(libDir, moduleDir));
    promises.push(buildCodemirror(libDir, moduleDir));
    promises.push(copy.glob(filesDir + "/diff_match_patch/**/*", filesDir, libDir));

    //Return composite promise
    return Promise.all(promises);
}

function buildCkEditor(outDir, moduleDir): Promise<any> {
    var promises: Promise<any>[] = [];

    //ckeditor
    promises.push(copy.glob(moduleDir + "/ckeditor/ckeditor.js", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/contents.css", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/skins/moono/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/lang/en.js", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/*.png", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/magicline/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/colorbutton/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/panelbutton/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/floatpanel/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/dialog/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/specialchar/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/clipboard/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/link/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/table/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/pastefromword/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/uploadwidget/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/notificationaggregator/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/filetools/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/widget/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/lineutils/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/notification/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/ckeditor/plugins/image/**/*", moduleDir, outDir));

    //External plugins
    promises.push(copy.glob(moduleDir + "/ckeditor-youtube-plugin/youtube/**/*", moduleDir + '/ckeditor-youtube-plugin', outDir + "/ckeditor/plugins/"));

    //Our customizations
    promises.push(copy.glob(filesDir + "/ckeditor/**/*", filesDir, outDir));

    return Promise.all(promises);
}

function buildCodemirror(outDir, moduleDir): Promise<any> {
    var promises: Promise<any>[] = [];

    //Codemirror
    promises.push(copy.glob(moduleDir + "/codemirror/lib/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/codemirror/mode/xml/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/codemirror/mode/javascript/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/codemirror/mode/css/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/codemirror/mode/htmlmixed/**/*", moduleDir, outDir));
    promises.push(copy.glob(moduleDir + "/codemirror/addon/merge/**/*", moduleDir, outDir));

    //Our Customizations
    promises.push(copy.glob(filesDir + "/codemirror/**/*", filesDir, outDir));

    return Promise.all(promises);
}