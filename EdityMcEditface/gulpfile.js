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
            "./custom_components/diff_match_patch/**/*",
            "./custom_components/ckeditor/**/*",
            "./custom_components/edity/**/*",
            "!./custom_components/edity/**/*.less",
            "./custom_components/codemirror/**/*",
            //"./custom_components/jsns/jsns.min.js",
            "!**/*.intellisense.js",
            "!**/*.less"],
        baseName: './custom_components',
        dest: libDir
    });

    htmlRapierBuild(__dirname, libDir, sharedSettings);
    htmlRapierWidgetsBuild(__dirname, libDir, sharedSettings);
    htmlRapierBootstrapBuild(__dirname, libDir, sharedSettings);

    compileLess({
        files: [
        __dirname + '/custom_components/bootstrap/bootstrap-custom.less'
        ],
        dest: libDir + '/bootstrap/dist/css',
        importPaths: path.join(__dirname),
    });

    compileLess({
        files: [
        __dirname + '/custom_components/edity/**/*.less'
        ],
        dest: libDir + '/edity',
        importPaths: path.join(__dirname),
    });
    
    //compileJsnsTs({
    //    libs: [
    //        __dirname + "/custom_components/edity/**/*.ts",
    //        "!**/*.intellisense.js"
    //    ],
    //    runners: false,
    //    output: "services",
    //    dest: libDir + '/edity',
    //    sourceRoot: __dirname + "/custom_components/edity/"
    //});

    //Client Side ts
    compileTypescript({
        libs: [
            __dirname + "/Client/Libs/**/*.ts",
        ],
        runners: ["edity.config"],
        dest: libDir,
        sourceRoot: __dirname + "/Client/Libs/",
        namespace: "clientlibs",
        output: "ClientLibs",
        concat: sharedSettings.concat,
        minify: sharedSettings.minify
    });

    //Client side views
    var viewBaseDir = webroot + "/edity";

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
        //output: "EdityWidgets",
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