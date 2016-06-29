﻿/// <binding BeforeBuild='copynodelibs, copybowerlibs' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    browserify = require("browserify"),
    source = require('vinyl-source-stream');

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

gulp.task("dobrowserify", function () {
    return browserify(webroot + "Sample.js")
        .bundle()
        .pipe(source("Sample.cmp.js"))
        .pipe(gulp.dest(webroot));
});

gulp.task("moretests", function () {
    return browserify("./GulpLibs/jquery/jquery.js")
        .bundle()
        .pipe(source("lib/jquery/jquery.js"))
        .pipe(gulp.dest(webroot));
});

var nodeLibs = [
    "./node_modules/jquery/dist/**/*",
    "./node_modules/bootstrap/dist/**/*",
    "./node_modules/htmlrest/src/**/*",
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