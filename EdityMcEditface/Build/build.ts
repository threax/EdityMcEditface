import * as edityCoreBuild from 'editymceditface.client/Build/build';
import * as clientBuild from 'editymceditface.client/Build/clientbuild';

var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');
var tsc = require('threax-npm-tk/typescript').tsc;
var jsnsTools = require('threax-npm-tk/jsnstools');

var filesDir = __dirname + "/..";

build(filesDir + "/ClientBin/Site", filesDir + "/wwwroot", filesDir + "/node_modules");

export function build(outDir, iconOutPath, moduleDir): Promise<any> {
    var promises = [];

    promises.push(edityCoreBuild.build(filesDir + "/ClientBin/EdityMcEditface", filesDir + "/wwwroot", filesDir + "/node_modules"));

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

    promises.push(compileTypescript());

    //Return composite promise
    return Promise.all(promises);
}

async function compileTypescript() {
    await tsc({
        projectFolder: filesDir
    });

    jsnsTools.saveLoadedModules(filesDir + '/ClientBin/Site/lib/tsbin.js', [], filesDir + '/ClientBin/Site/lib/tsbin.prod.js')
}