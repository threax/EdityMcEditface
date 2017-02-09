"use strict";

var path = require('path');

var compileTypescript = require('threax-gulp-tk/typescript.js');
var compileLess = require('threax-gulp-tk/less.js');
var copyFiles = require('threax-gulp-tk/copy.js');


module.exports = function (sharedSettings, inputDir, outputDir, clientSideNamespace) {
    //Spc specific client side
    var childClientDir = inputDir;
    var libDir = outputDir + "/lib";
    var viewBaseDir = outputDir + "/edity";

    //less
    compileLess({
        files: [
        childClientDir + '/Less/**/*.less'
        ],
        dest: libDir + '/css',
        importPaths: [path.join("node_modules/bootstrap/less")],
    });

    //Client Side ts
    compileTypescript({
        libs: [
            childClientDir + "/Libs/**/*.ts",
        ],
        runners: false,
        dest: libDir,
        sourceRoot: childClientDir + "/Libs/",
        namespace: clientSideNamespace,
        output: clientSideNamespace,
        concat: sharedSettings.concat,
        minify: sharedSettings.minify
    });

    //Compile view typescript
    compileTypescript({
        libs: [
            childClientDir + "/Views/**/*.ts",
            "!**/*.intellisense.js"
        ],
        runners: true,
        dest: viewBaseDir + '/layouts',
        sourceRoot: childClientDir + "/Views/"
    });

    //Copy view files
    copyFiles({
        libs: [
            childClientDir + "/Views/**/*.html",
            childClientDir + "/Views/**/*.js",
            childClientDir + "/Views/**/*.json",
            childClientDir + "/Views/**/*.css",
            "!**/*.intellisense.js"
        ],
        baseName: childClientDir + "/Views",
        dest: viewBaseDir + '/layouts'
    });

    copyFiles({
        libs: [
            childClientDir + "/Templates/**/*.html",
            childClientDir + "/Templates/**/*.js",
            childClientDir + "/Templates/**/*.json",
            childClientDir + "/Templates/**/*.css",
            "!**/*.intellisense.js"
        ],
        baseName: childClientDir + "/Views",
        dest: viewBaseDir + '/templates'
    });

    copyFiles({
        libs: [
            childClientDir + "/edity.json",
        ],
        baseName: childClientDir,
        dest: viewBaseDir
    });
}