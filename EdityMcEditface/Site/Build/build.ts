"use strict";
import * as clientBuild from './../../Client/Build/clientbuild';
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');

var filesDir = __dirname + "/..";

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