"use strict";

var gulp = require("gulp");
var path = require('path');

var compileTypescript = require('threax-gulp-tk/typescript.js');
var compileLess = require('threax-gulp-tk/less.js');
var copyFiles = require('threax-gulp-tk/copy.js');

module.exports = function (rootDir, webroot, settings) {

    var htmlRapierBuild = require(rootDir + '/node_modules/htmlrapier/build');
    var htmlRapierWidgetsBuild = require(rootDir + '/node_modules/htmlrapier.widgets/build');
    var htmlRapierBootstrapBuild = require(rootDir + '/node_modules/htmlrapier.bootstrap/build');

    var libDir = webroot + "/lib";
    var viewBaseDir = webroot + "/edity";
    var editylibDir = viewBaseDir + "/lib";

    if (settings === undefined) {
        settings = {};
    }

    var concat = true;
    if (settings.concat !== undefined) {
        concat = settings.concat;
    }

    var minify = true;
    if (settings.minify !== undefined) {
        minify = settings.minify;
    }

    //Shared client side
    copyFiles({
        libs: ["./node_modules/jsns/jsns.js",
        ],
        baseName: './node_modules/jsns',
        dest: libDir
    });

    htmlRapierBuild(__dirname, libDir, settings);
    htmlRapierWidgetsBuild(__dirname, libDir, settings);
    htmlRapierBootstrapBuild(__dirname, libDir, settings);

    compileLess({
        files: [
        __dirname + '/bootstrap/bootstrap-custom.less'
        ],
        dest: libDir + '/bootstrap/dist/css',
        importPaths: path.join(rootDir + '/node_modules/bootstrap/less'),
    });

    copyFiles({
        libs: [rootDir + "/node_modules/bootstrap/dist/**/*",
               "!" + rootDir + "/node_modules/bootstrap/dist/css/**/*",
               "!" + rootDir + "/node_modules/bootstrap/dist/js/**/*"
        ],
        baseName: rootDir + '/node_modules',
        dest: libDir
    });

    compileLess({
        files: [
        __dirname + '/edity/**/*.less'
        ],
        dest: libDir + '/edity',
        importPaths: path.join(__dirname),
    });

    //Compile widget typescript
    compileTypescript({
        libs: [
            __dirname + "/Widgets/**/*.ts",
            "!**/*.intellisense.js"
        ],
        runners: false,
        dest: libDir + '/Widgets',
        sourceRoot: __dirname + "/Widgets/",
        namespace: "edity.widgets",
        concat: false,
        minify: settings.minify
    });

    //Edity Editor Client Side
    copyFiles({
        libs: [rootDir + "/node_modules/codemirror/lib/**/*",
               rootDir + "/node_modules/codemirror/mode/xml/**/*",
               rootDir + "/node_modules/codemirror/mode/javascript/**/*",
               rootDir + "/node_modules/codemirror/mode/css/**/*",
               rootDir + "/node_modules/codemirror/mode/htmlmixed/**/*",
               rootDir + "/node_modules/codemirror/addon/merge/**/*" 
        ],
        baseName: rootDir + '/node_modules',
        dest: editylibDir
    });

    copyFiles({
        libs: [rootDir + "/node_modules/ckeditor/ckeditor.js",
               rootDir + "/node_modules/ckeditor/contents.css",
               rootDir + "/node_modules/ckeditor/skins/moono/**/*",
               rootDir + "/node_modules/ckeditor/lang/en.js",
               rootDir + "/node_modules/ckeditor/plugins/*.png",
               rootDir + "/node_modules/ckeditor/plugins/magicline/**/*",
               rootDir + "/node_modules/ckeditor/plugins/colorbutton/**/*",
               rootDir + "/node_modules/ckeditor/plugins/panelbutton/**/*",
               rootDir + "/node_modules/ckeditor/plugins/floatpanel/**/*",
               rootDir + "/node_modules/ckeditor/plugins/dialog/**/*",
               rootDir + "/node_modules/ckeditor/plugins/specialchar/**/*",
               rootDir + "/node_modules/ckeditor/plugins/clipboard/**/*",
               rootDir + "/node_modules/ckeditor/plugins/link/**/*",
               rootDir + "/node_modules/ckeditor/plugins/table/**/*",
               rootDir + "/node_modules/ckeditor/plugins/pastefromword/**/*",
               rootDir + "/node_modules/ckeditor/plugins/uploadwidget/**/*",
               rootDir + "/node_modules/ckeditor/plugins/notificationaggregator/**/*",
               rootDir + "/node_modules/ckeditor/plugins/filetools/**/*",
               rootDir + "/node_modules/ckeditor/plugins/widget/**/*",
               rootDir + "/node_modules/ckeditor/plugins/lineutils/**/*",
               rootDir + "/node_modules/ckeditor/plugins/notification/**/*",
               rootDir + "/node_modules/ckeditor/plugins/image/**/*",
        ],
        baseName: rootDir + '/node_modules',
        dest: editylibDir
    });

    copyFiles({
        libs: [rootDir + "/node_modules/ckeditor-youtube-plugin/youtube/**/*"],
        baseName: rootDir + '/node_modules/ckeditor-youtube-plugin',
        dest: editylibDir + "/ckeditor/plugins/"
    });

    copyFiles({
        libs: [
            __dirname + "/diff_match_patch/**/*",
            __dirname + "/ckeditor/**/*",
            __dirname + "/codemirror/**/*",
            "!**/*.intellisense.js",
            "!**/*.less"],
        baseName: __dirname,
        dest: editylibDir
    });

    //Editor Core ts
    compileTypescript({
        libs: [
            __dirname + "/EditorCore/**/*.ts",
            "!" + __dirname + "/EditorCore/EditorPageStart.ts",
        ],
        runners: ["edity.config"],
        dest: editylibDir,
        sourceRoot: __dirname + "/EditorCore/",
        namespace: "edity.editorcore",
        output: "EditorCore",
        concat: settings.concat,
        minify: settings.minify
    });

    compileTypescript({
        libs: [
            __dirname + "/EditorCore/EditorPageStart.ts",
        ],
        runners: null,
        dest: editylibDir,
        sourceRoot: __dirname + "/EditorCore/",
        namespace: "edity.editorcore",
        output: "EditorCorePageStart",
        concat: settings.concat,
        minify: settings.minify
    });

    //Compile view typescript
    compileTypescript({
        libs: [
            __dirname + "/Views/**/*.ts",
            "!**/*.intellisense.js"
        ],
        runners: true,
        dest: viewBaseDir + '/layouts',
        sourceRoot: __dirname + "/Views/"
    });

    //Copy view files
    //Not working currently, converts everything to ascii for some reason, lots of version changes for node and gulp incliding 7 and 4 respectivly did not fix
    copyFiles({
        libs: [
            __dirname + "/Views/**/*.html",
            __dirname + "/Views/**/*.js",
            __dirname + "/Views/**/*.json",
            __dirname + "/Views/**/*.css",
            "!**/*.intellisense.js"
        ],
        baseName: __dirname + "/Views",
        dest: viewBaseDir + '/layouts'
    });

    copyFiles({
        libs: [
            __dirname + "/Templates/**/*.html",
            __dirname + "/Templates/**/*.js",
            __dirname + "/Templates/**/*.json",
            __dirname + "/Templates/**/*.css",
            "!**/*.intellisense.js"
        ],
        baseName: __dirname + "/Views",
        dest: viewBaseDir + '/templates'
    });

    copyFiles({
        libs: [
            __dirname + "/edity.json"
        ],
        baseName: __dirname,
        dest: viewBaseDir
    });
};