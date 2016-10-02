var jsnsModuleify = require('threax-gulp-tk/umd.js');
var compileJavascript = require('threax-gulp-tk/javascript.js');
var gulp = require('gulp');
var runSequence = require('run-sequence').use(gulp);

function compile(sourceDir, destDir) {

    sourceDir += '/lib';
    console.log("Compiling bootstrap.native");

    var taskList = [];
    quickTask('thednp.bootstrap.native.affix', "affix-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.alert', "alert-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.button', "button-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.carousel', "carousel-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.collapse', "collapse-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.dropdown', "dropdown-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.modal', "modal-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.popover', "popover-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.scrollspy', "scrollspy-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.tab', "tab-native.js", sourceDir, taskList);
    quickTask('thednp.bootstrap.native.tooltip', "tooltip-native.js", sourceDir, taskList);

    var jsTask = uniqueTask("compile-bootstrap.native");

    gulp.task(jsTask, function () {
        return compileJavascript({
            libs: [
                sourceDir + "/utils.js",
                __dirname + "/modules/**/*.js",
                __dirname + "/plugin.js",
            ],
            output: "bootstrap.native",
            dest: destDir,
            sourceRoot: __dirname + "/modules/",
            minify: true,
            concat: true
        });
    });

    runSequence(taskList, jsTask);
}

var taskUnique = 0;

function uniqueTask(taskName) {
    return taskName + '_' + taskUnique++;
}

function quickTask(moduleName, sourceFile, sourceDir, taskList) {
    var taskName = uniqueTask(moduleName);
    taskList.push(taskName);
    gulp.task(taskName, function () {
        return quickModule(moduleName, sourceFile, sourceDir);
    });
}

function quickModule(moduleName, sourceFile, sourceDir) {
    var settings = {
        moduleName: moduleName,
        libs: [
            sourceDir + '/' + sourceFile
        ],
        output: sourceFile,
        dest: __dirname + "/modules/",
        sourceRoot: sourceDir + '\\',
    };

    return jsnsModuleify(settings);
}

module.exports = compile;