/// <binding BeforeBuild='copynodelibs, copybowerlibs' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

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

var nodeLibs = [
    "./node_modules/jquery/dist/**/*",
    "./node_modules/bootstrap/dist/**/*",
    "./node_modules/htmlrest/src/**/*",
    "./node_modules/json-editor/dist/**/*",
    "./node_modules/ckeditor/ckeditor.js"
];

gulp.task("copynodelibs", function () {
    return gulp.src(nodeLibs, { base: './node_modules' })
        .pipe(gulp.dest(webroot + "lib"));
});

var bowerLibs = [
    "./bower_components/pen/src/**/*"
];

gulp.task("copybowerlibs", function () {
    return gulp.src(bowerLibs, { base: './bower_components' })
        .pipe(gulp.dest(webroot + "lib"));
});