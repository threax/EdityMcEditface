"use strict";
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');
var clientBuild = require('./clientbuild');

module.exports = function (outDir, iconOutPath, moduleDir) {

    var libDir = outDir;

    var promises = [];

    promises.push(clientBuild(outDir, iconOutPath, moduleDir));

    promises.push(copy.glob(
        moduleDir + "/codemirror/lib/**/*",
        moduleDir,
        libDir + '/edity/lib'
    ));

//    //Edity Editor Client Side
//    copyFiles({
//        libs: [rootDir + "/node_modules/codemirror/lib/**/*",
//               rootDir + "/node_modules/codemirror/mode/xml/**/*",
//               rootDir + "/node_modules/codemirror/mode/javascript/**/*",
//               rootDir + "/node_modules/codemirror/mode/css/**/*",
//               rootDir + "/node_modules/codemirror/mode/htmlmixed/**/*",
//               rootDir + "/node_modules/codemirror/addon/merge/**/*" 
//        ],
//        baseName: rootDir + '/node_modules',
//        dest: editylibDir
//    });

//    copyFiles({
//        libs: [rootDir + "/node_modules/ckeditor/ckeditor.js",
//               rootDir + "/node_modules/ckeditor/contents.css",
//               rootDir + "/node_modules/ckeditor/skins/moono/**/*",
//               rootDir + "/node_modules/ckeditor/lang/en.js",
//               rootDir + "/node_modules/ckeditor/plugins/*.png",
//               rootDir + "/node_modules/ckeditor/plugins/magicline/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/colorbutton/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/panelbutton/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/floatpanel/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/dialog/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/specialchar/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/clipboard/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/link/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/table/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/pastefromword/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/uploadwidget/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/notificationaggregator/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/filetools/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/widget/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/lineutils/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/notification/**/*",
//               rootDir + "/node_modules/ckeditor/plugins/image/**/*",
//        ],
//        baseName: rootDir + '/node_modules',
//        dest: editylibDir
//    });

//    copyFiles({
//        libs: [rootDir + "/node_modules/ckeditor-youtube-plugin/youtube/**/*"],
//        baseName: rootDir + '/node_modules/ckeditor-youtube-plugin',
//        dest: editylibDir + "/ckeditor/plugins/"
//    });

//    copyFiles({
//        libs: [
//            __dirname + "/diff_match_patch/**/*",
//            __dirname + "/ckeditor/**/*",
//            __dirname + "/codemirror/**/*",
//            "!**/*.intellisense.js",
//            "!**/*.less"],
//        baseName: __dirname,
//        dest: editylibDir
//    });

    return Promise.all(promises);
}