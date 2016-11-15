"use strict";

var gulp = require("gulp");
var path = require('path');

var compileTypescript = require('threax-gulp-tk/typescript.js');
var compileLess = require('threax-gulp-tk/less.js');
var copyFiles = require('threax-gulp-tk/copy.js');

var htmlRapierBuild = require(__dirname + '/node_modules/htmlrapier/build');
var htmlRapierWidgetsBuild = require(__dirname + '/node_modules/htmlrapier.widgets/build');
var htmlRapierBootstrapBuild = require(__dirname + '/node_modules/htmlrapier.bootstrap/build');

var webroot = __dirname + "/wwwroot";

gulp.task("default",function () {
    build();
});

gulp.task("debug",function () {
    build({
        minify:false
    });
});

function build(sharedSettings) {
    if (sharedSettings === undefined) {
        sharedSettings = {};
    }

    if (sharedSettings.minify === undefined) {
        sharedSettings.minify = true;
    }

    if (sharedSettings.concat === undefined) {
        sharedSettings.concat = true;
    }

    var libDir = webroot + "/lib";

    copyFiles({
        libs: ["./node_modules/jquery/dist/**/*",
               "./node_modules/bootstrap/dist/**/*",
               "!./node_modules/bootstrap/dist/css/**/*",
               "./node_modules/codemirror/lib/**/*",
               "./node_modules/codemirror/mode/xml/**/*",
               "./node_modules/codemirror/mode/javascript/**/*",
               "./node_modules/codemirror/mode/css/**/*",
               "./node_modules/codemirror/mode/htmlmixed/**/*",
               "./node_modules/codemirror/addon/merge/**/*",
               "./node_modules/ckeditor/ckeditor.js",
               "./node_modules/ckeditor/contents.css",
               "./node_modules/ckeditor/skins/moono/**/*",
               "./node_modules/ckeditor/lang/en.js",
               "./node_modules/ckeditor/plugins/*.png",
               "./node_modules/ckeditor/plugins/magicline/**/*",
               "./node_modules/ckeditor/plugins/colorbutton/**/*",
               "./node_modules/ckeditor/plugins/panelbutton/**/*",
               "./node_modules/ckeditor/plugins/floatpanel/**/*",
               "./node_modules/ckeditor/plugins/dialog/**/*",
               "./node_modules/ckeditor/plugins/specialchar/**/*",
               "./node_modules/ckeditor/plugins/clipboard/**/*",
               "./node_modules/ckeditor/plugins/link/**/*",
               "./node_modules/ckeditor/plugins/table/**/*",
               "./node_modules/ckeditor/plugins/pastefromword/**/*",
               "./node_modules/ckeditor/plugins/uploadwidget/**/*",
               "./node_modules/ckeditor/plugins/uploadimage/**/*",
               "./node_modules/ckeditor/plugins/notificationaggregator/**/*",
               "./node_modules/ckeditor/plugins/filetools/**/*",
               "./node_modules/ckeditor/plugins/widget/**/*",
               "./node_modules/ckeditor/plugins/lineutils/**/*",
               "./node_modules/ckeditor/plugins/notification/**/*",
               "./node_modules/jsns/jsns.js",
               "./node_modules/ckeditor/plugins/image/**/*", 
        ],
        baseName: './node_modules',
        dest: libDir
    });

    copyFiles({
        libs: ["./node_modules/ckeditor-youtube-plugin/youtube/**/*"],
        baseName: './node_modules/ckeditor-youtube-plugin',
        dest: libDir + "/ckeditor/plugins/"
    });

    copyFiles({
        libs: [
            "./Client/diff_match_patch/**/*",
            "./Client/ckeditor/**/*",
            "./Client/edity/**/*",
            "!./Client/edity/**/*.less",
            "./Client/codemirror/**/*",
            "!**/*.intellisense.js",
            "!**/*.less"],
        baseName: './Client',
        dest: libDir
    });

    htmlRapierBuild(__dirname, libDir, sharedSettings);
    htmlRapierWidgetsBuild(__dirname, libDir, sharedSettings);
    htmlRapierBootstrapBuild(__dirname, libDir, sharedSettings);

    compileLess({
        files: [
        __dirname + '/Client/bootstrap/bootstrap-custom.less'
        ],
        dest: libDir + '/bootstrap/dist/css',
        importPaths: path.join(__dirname),
    });

    compileLess({
        files: [
        __dirname + '/Client/edity/**/*.less'
        ],
        dest: libDir + '/edity',
        importPaths: path.join(__dirname),
    });

    //Client side
    var viewBaseDir = webroot + "/edity";

    //Editor Core ts
    compileTypescript({
        libs: [
            __dirname + "/Client/EditorCore/**/*.ts",
        ],
        runners: ["edity.config"],
        dest: viewBaseDir,
        sourceRoot: __dirname + "/Client/EditorCore/",
        namespace: "edity.editorcore",
        output: "EditorCore",
        concat: sharedSettings.concat,
        minify: sharedSettings.minify
    });

    //Compile view typescript
    compileTypescript({
        libs: [
            __dirname + "/Client/Views/**/*.ts",
            "!**/*.intellisense.js"
        ],
        runners: true,
        dest: viewBaseDir + '/layouts',
        sourceRoot: __dirname + "/Client/Views/"
    });

    //Compile widget typescript
    compileTypescript({
        libs: [
            __dirname + "/Client/Widgets/**/*.ts",
            "!**/*.intellisense.js"
        ],
        runners: false,
        dest: libDir + '/Widgets',
        sourceRoot: __dirname + "/Client/Widgets/",
        namespace: "edity.widgets",
        concat: false,
        minify: sharedSettings.minify
    });

    //Copy view files
    //Not working currently, converts everything to ascii for some reason, lots of version changes for node and gulp incliding 7 and 4 respectivly did not fix
    copyFiles({
        libs: [
            __dirname + "/Client/Views/**/*.html",
            __dirname + "/Client/Views/**/*.js",
            __dirname + "/Client/Views/**/*.json",
            __dirname + "/Client/Views/**/*.css",
            "!**/*.intellisense.js"
        ],
        baseName: __dirname + "/Client/Views",
        dest: viewBaseDir + '/layouts'
    });

    copyFiles({
        libs: [
            __dirname + "/Client/Templates/**/*.html",
            __dirname + "/Client/Templates/**/*.js",
            __dirname + "/Client/Templates/**/*.json",
            __dirname + "/Client/Templates/**/*.css",
            "!**/*.intellisense.js"
        ],
        baseName: __dirname + "/Client/Views",
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