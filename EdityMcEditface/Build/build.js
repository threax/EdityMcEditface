"use strict";
var edityCoreBuild = require("editymceditface.client/Build/build");
var siteBuild = require("./../Site/Build/build");
var tsc = require('threax-npm-tk/typescript');
var filesDir = __dirname + "/..";
edityCoreBuild.build(filesDir + "/ClientBin/EdityMcEditface", filesDir + "/wwwroot", filesDir + "/node_modules");
siteBuild.build(filesDir + "/ClientBin/Site", filesDir + "/wwwroot", filesDir + "/node_modules");
tsc({
    projectFolder: filesDir
});
//# sourceMappingURL=build.js.map