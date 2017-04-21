import * as edityCoreBuild from 'editymceditface.client/Build/build';
import * as clientBuild from 'editymceditface.client/Build/clientbuild';

var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');
var tsc = require('threax-npm-tk/typescript').tsc;

var filesDir = __dirname + "/..";

edityCoreBuild.build(filesDir + "/ClientBin/EdityMcEditface", filesDir + "/wwwroot", filesDir + "/node_modules");
build(filesDir + "/ClientBin/Site", filesDir + "/wwwroot", filesDir + "/node_modules");
tsc({
    projectFolder: filesDir
});

export function build(outDir, iconOutPath, moduleDir): Promise<any> {
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