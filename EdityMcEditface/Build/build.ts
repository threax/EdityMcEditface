import * as clientBuild from './../Client/Build/build';
var tsc = require('threax-npm-tk/typescript');

var filesDir = __dirname + "/..";

clientBuild.build(filesDir + "/ClientBin/EdityMcEditface", filesDir + "/wwwroot", filesDir + "/node_modules");
tsc();