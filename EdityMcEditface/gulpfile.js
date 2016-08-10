/// <binding BeforeBuild='copynodelibs, copybowerlibs' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename"),
    sourcemaps = require("gulp-sourcemaps");

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

gulp.task("copylibs", function () {
    var libDir = webroot + "lib/";

    copyFiles({
        libs: ["./node_modules/jquery/dist/**/*",
               "./node_modules/bootstrap/dist/**/*",
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
               "./node_modules/ckeditor/plugins/image/**/*",],
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
            "!**/*.intellisense.js"],
        baseName: './custom_components',
        dest: libDir
    });

    //Minify htmlrest, need to specify the load order for jsns to be first
    var htmlrestCompile = {
        libs: ["./custom_components/htmlrest/src/jsns.js",
               "./custom_components/htmlrest/src/polyfill.js",
               "./custom_components/htmlrest/src/**/*.js",
               "./custom_components/htmlrest/plugin/**/*",
               "./custom_components/htmlrest/widgets/**/*",
               "./custom_components/htmlrest/data/**/*",
               "!**/*.intellisense.js"],
        output: "htmlrest",
        dest: "./custom_components/htmlrest",
        //base: './custom_components/htmlrest',
        sourceRoot: "C:/Development/EdityMcEditface/EdityMcEditface/custom_components/htmlrest/src/"
    };

    minifyJs(htmlrestCompile);
    concatJs(htmlrestCompile);
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