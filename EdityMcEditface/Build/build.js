"use strict";
var edityCoreBuild = require("editymceditface.client/Build/build");
var clientBuild = require("editymceditface.client/Build/clientbuild");
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');
var tsc = require('threax-npm-tk/typescript');
var filesDir = __dirname + "/..";
edityCoreBuild.build(filesDir + "/ClientBin/EdityMcEditface", filesDir + "/wwwroot", filesDir + "/node_modules");
build(filesDir + "/ClientBin/Site", filesDir + "/wwwroot", filesDir + "/node_modules");
tsc({
    projectFolder: filesDir
});
function build(outDir, iconOutPath, moduleDir) {
    var promises = [];
    //Build client
    promises.push(clientBuild.build(outDir, iconOutPath, moduleDir, filesDir));
    //Build bootstrap theme
    promises.push(less.compile({
        encoding: 'utf8',
        importPaths: [moduleDir, moduleDir + '/bootstrap/less'],
        input: filesDir + '/bootstrap/bootstrap-custom.less',
        basePath: filesDir + '/bootstrap',
        out: outDir + "/lib/bootstrap/dist/css",
        compress: true,
    }));
    //Return composite promise
    return Promise.all(promises);
}
exports.build = build;
//# sourceMappingURL=build.js.map