"use strict";
var clientBuild = require("./clientbuild");
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');
var filesDir = __dirname + "/..";
function build(outDir, iconOutPath, moduleDir) {
    var promises = [];
    var libDir = outDir + '/edity/lib';
    promises.push(clientBuild.build(outDir, iconOutPath, moduleDir, filesDir));
    promises.push(buildCkEditor(libDir, moduleDir));
    promises.push(buildCodemirror(libDir, moduleDir));
    promises.push(buildBootstrap(outDir + '/lib', moduleDir));
    promises.push(copy.glob(filesDir + "/diff_match_patch/**/*", filesDir, libDir));
    //Return composite promise
    return Promise.all(promises);
}
exports.build = build;
function buildBootstrap(outDir, moduleDir) {
    var promises = [];
    //Build bootstrap theme
    promises.push(less.compile({
        encoding: 'utf8',
        importPaths: [filesDir, moduleDir + '/bootstrap/less'],
        input: filesDir + '/bootstrap/bootstrap-custom.less',
        basePath: filesDir + '/bootstrap',
        out: outDir + "/bootstrap/dist/css",
        compress: true,
    }));
    promises.push(copy.glob(moduleDir + "/bootstrap/dist/fonts/**/*", moduleDir + "/bootstrap/dist/fonts", outDir + "/bootstrap/dist/fonts"));
    return Promise.all(promises);
}
function buildCkEditor(outDir, moduleDir) {
    var promises = [];
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
function buildCodemirror(outDir, moduleDir) {
    var promises = [];
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
//# sourceMappingURL=build.js.map