"use strict";

var gulp = require("gulp");
var path = require('path');

var compileTypescript = require('threax-gulp-tk/typescript.js');
var compileLess = require('threax-gulp-tk/less.js');
var copyFiles = require('threax-gulp-tk/copy.js');

var htmlRapierBuild = require(__dirname + '/node_modules/htmlrapier/build');
var htmlRapierWidgetsBuild = require(__dirname + '/node_modules/htmlrapier.widgets/build');
var htmlRapierBootstrapBuild = require(__dirname + '/node_modules/htmlrapier.bootstrap/build');
var treeMenuBuild = require(__dirname + '/node_modules/htmlrapier.treemenu/build');
var clientBuild = require(__dirname + '/Client/build');

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
    var viewBaseDir = webroot + "/edity";
    var editylibDir = viewBaseDir + "/lib";

    //Shared client side
    copyFiles({
        libs: ["./node_modules/jsns/jsns.js",
        ],
        baseName: './node_modules/jsns',
        dest: libDir
    });

    htmlRapierBuild(__dirname, libDir, sharedSettings);
    htmlRapierWidgetsBuild(__dirname, libDir, sharedSettings);
    htmlRapierBootstrapBuild(__dirname, libDir, sharedSettings);
    treeMenuBuild(__dirname, libDir, sharedSettings);
    clientBuild(__dirname, libDir, viewBaseDir, editylibDir, sharedSettings);
};