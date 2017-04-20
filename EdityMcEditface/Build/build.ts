import * as edityCoreBuild from 'editymceditface.client/Build/build';
import * as siteBuild from './../Site/Build/build';
var tsc = require('threax-npm-tk/typescript');

var filesDir = __dirname + "/..";

edityCoreBuild.build(filesDir + "/ClientBin/EdityMcEditface", filesDir + "/wwwroot", filesDir + "/node_modules");
siteBuild.build(filesDir + "/ClientBin/Site", filesDir + "/wwwroot", filesDir + "/node_modules");
tsc({
    projectFolder: filesDir
});