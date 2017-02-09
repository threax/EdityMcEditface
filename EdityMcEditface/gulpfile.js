"use strict";

var gulp = require("gulp");
var path = require('path');

var compileTypescript = require('threax-gulp-tk/typescript.js');
var compileLess = require('threax-gulp-tk/less.js');
var copyFiles = require('threax-gulp-tk/copy.js');

var treeMenuBuild = require(__dirname + '/node_modules/htmlrapier.treemenu/build');
var edityClientBuild = require(__dirname + '/Client/build');
var siteClientBuild = require(__dirname + '/Client/clientbuild');

var webroot = __dirname + "/ClientBin";

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

    var edityOuputRoot = webroot + "/EdityMcEditface";
    var siteOutputRoot = webroot + "/Site"
    var libDir = siteOutputRoot + "/lib";

    edityClientBuild(__dirname, edityOuputRoot, sharedSettings);
    siteClientBuild(sharedSettings, __dirname + "/Site", siteOutputRoot, "site.client");
    treeMenuBuild(__dirname, libDir, sharedSettings);
};