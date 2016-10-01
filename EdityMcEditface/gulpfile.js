﻿/// <binding Clean='clean' ProjectOpened='watchers' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename"),
    sourcemaps = require("gulp-sourcemaps");
var less = require('gulp-less');
var path = require('path');
var uglifycss = require('gulp-uglifycss');
var gutil = require('gulp-util');
var plumber = require('gulp-plumber');

var htmlRapierBuild = require(__dirname + '/custom_components/HtmlRapier/build');
var htmlRapierWidgetsBuild = require(__dirname + '/custom_components/HtmlRapierWidgets/build');

var webroot = "./wwwroot/";

var paths = {
    js: webroot + "js/**/*.js",
    minJs: webroot + "js/**/*.min.js",
    css: webroot + "css/**/*.css",
    minCss: webroot + "css/**/*.min.css",
    concatJsDest: webroot + "js/site.min.js",
    concatCssDest: webroot + "css/site.min.css"
};

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("default", function () {
    var libDir = webroot + "lib/";

    copyFiles({
        libs: ["./node_modules/jquery/dist/**/*",
               "./node_modules/bootstrap/dist/**/*",
               "!./node_modules/bootstrap/dist/css/**/*",
               "./node_modules/bootstrap.native/dist/**/*",
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
               "./node_modules/ckeditor/plugins/image/**/*", 
        ],//"./node_modules/jsns/jsns.min.js"
        baseName: './node_modules',
        dest: libDir
    });

    copyFiles({
        libs: ["./node_modules/ckeditor-youtube-plugin/youtube/**/*"],
        baseName: './node_modules/ckeditor-youtube-plugin',
        dest: libDir + "ckeditor/plugins/"
    });

    copyFiles({
        libs: [
            "./custom_components/diff_match_patch/**/*",
            "./custom_components/ckeditor/**/*",
            "./custom_components/edity/**/*",
            "./custom_components/codemirror/**/*",
            "./custom_components/jsns/jsns.js",
            "!**/*.intellisense.js",
            "!**/*.less"],
        baseName: './custom_components',
        dest: libDir
    });

    htmlRapierBuild(__dirname, __dirname + "/wwwroot/lib/HtmlRapier");
    htmlRapierWidgetsBuild(__dirname, __dirname + "/wwwroot/lib/HtmlRapierWidgets");

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
});

var watchFiles = ['./custom_components/**/*.js', '!./custom_components/HtmlRapier/HtmlRapier.js', '!./custom_components/HtmlRapier/HtmlRapier.min.js', './custom_components/**/*.less'];
gulp.task('watchers', function () {
    gulp.watch(watchFiles, ['default']);
});

function copyFiles(settings) {
    gulp.src(settings.libs, { base: settings.baseName })
        .pipe(gulp.dest(settings.dest));
}

function minifyJs(settings) {
    return gulp.src(settings.libs, { base: settings.base })
        .pipe(sourcemaps.init())
        .pipe(concat(settings.output + '.js'))
        .pipe(uglify())
        .pipe(rename(settings.output + '.min.js'))
        .pipe(sourcemaps.write(".", { includeContent: false, sourceRoot: settings.sourceRoot }))
        .pipe(gulp.dest(settings.dest));
};

function concatJs(settings) {
    return gulp.src(settings.libs, { base: settings.base })
        .pipe(sourcemaps.init())
        .pipe(concat(settings.output + '.js'))
        .pipe(sourcemaps.write(".", { includeContent: false, sourceRoot: settings.sourceRoot }))
        .pipe(gulp.dest(settings.dest));
};

function compileLess(settings) {
    return gulp.src(settings.files, { base: settings.baseName })
           .pipe(plumber(function (error) {
               gutil.log(gutil.colors.red(error.message));
               gutil.beep();
               this.emit('end');
           }))
           .pipe(less({
               paths: settings.importPaths
           }))
           .pipe(uglifycss({
               "maxLineLen": 80,
               "uglyComments": true
           }))
           .pipe(gulp.dest(settings.dest));
}