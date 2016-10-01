var jsnsModuleify = require('threax-gulp-tk/umd.js');
var compileJavascript = require('threax-gulp-tk/javascript.js');

function compile(sourceDir, destDir) {

    sourceDir += '/lib'

    quickModule('thednp.bootstrap.native.affix', "affix-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.alert', "alert-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.button', "button-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.carousel', "carousel-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.collapse', "collapse-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.dropdown', "dropdown-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.modal', "modal-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.popover', "popover-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.scrollspy', "scrollspy-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.tab', "tab-native.js", sourceDir);
    quickModule('thednp.bootstrap.native.tooltip', "tooltip-native.js", sourceDir);

    compileJavascript({
        libs: [
            __dirname + "/modules/**/*.js",
            __dirname + "/plugin.js",
        ],
        output: "bootstrap.native",
        dest: destDir,
        sourceRoot: __dirname + "/modules/",
        minify: true,
        concat: true
    });
}

function quickModule(moduleName, sourceFile, sourceDir) {
    return jsnsModuleify({
        moduleName: moduleName,
        libs: [
            sourceDir + '/' + sourceFile
        ],
        output: sourceFile,
        dest: __dirname + "/modules/",
        sourceRoot: sourceDir + '\\',
    });
}

module.exports = compile;