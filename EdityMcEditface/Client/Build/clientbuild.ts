"use strict";
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');

var rootDir = __dirname + '/..'

export function build(outDir, iconOutPath, moduleDir): Promise<any> {
    var promises = [];

    promises.push(less({
        encoding: 'utf8',
        importPaths: [rootDir, moduleDir + '/bootstrap/less'],
        input: rootDir + '/edity/**/*.less',
        basePath: rootDir + '/edity',
        out: outDir + '/edity',
        compress: true,
    }));

    promises.push(less({
        encoding: 'utf8',
        importPaths: [rootDir, moduleDir + '/bootstrap/less'],
        input: rootDir + '/Views/**/*.less',
        basePath: rootDir + '/Views',
        out: outDir + '/edity/layouts',
        compress: true,
    }));

    //Views
    promises.push(copy.glob(
        rootDir + '/Views/**/*.css',
        rootDir + '/Views',
        outDir + '/edity/layouts'
    ));

    promises.push(copy.glob(
        rootDir + '/Views/**/*.html',
        rootDir + '/Views',
        outDir + '/edity/layouts'
    ));

    //Templates
    promises.push(copy.glob(
        rootDir + '/Templates/**/*.css',
        rootDir + '/Templates',
        outDir + '/edity/Templates'
    ));

    promises.push(copy.glob(
        rootDir + '/Templates/**/*.html',
        rootDir + '/Templates',
        outDir + '/edity/Templates'
    ));

    return Promise.all(promises);
}