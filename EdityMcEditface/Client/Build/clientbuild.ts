"use strict";
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');

export function build(outDir, iconOutPath, moduleDir, filesDir): Promise<any> {
    var promises = [];

    promises.push(less.compile({
        encoding: 'utf8',
        importPaths: [filesDir, moduleDir + '/bootstrap/less'],
        input: filesDir + '/edity/**/*.less',
        basePath: filesDir + '/edity',
        out: outDir + '/lib/edity',
        compress: true,
    }));

    //Views
    promises.push(copy.glob(
        filesDir + '/Views/**/*.css',
        filesDir + '/Views',
        outDir + '/edity/layouts'
    ));

    promises.push(copy.glob(
        filesDir + '/Views/**/*.html',
        filesDir + '/Views',
        outDir + '/edity/layouts'
    ));

    promises.push(copy.glob(
        filesDir + '/Views/**/*.json',
        filesDir + '/Views',
        outDir + '/edity/layouts'
    ));

    promises.push(less.compile({
        encoding: 'utf8',
        importPaths: [filesDir, moduleDir + '/bootstrap/less'],
        input: filesDir + '/Views/**/*.less',
        basePath: filesDir + '/Views',
        out: outDir + '/edity/layouts',
        compress: true,
    }));

    //Templates
    promises.push(copy.glob(
        filesDir + '/Templates/**/*.css',
        filesDir + '/Templates',
        outDir + '/edity/Templates'
    ));

    promises.push(copy.glob(
        filesDir + '/Templates/**/*.html',
        filesDir + '/Templates',
        outDir + '/edity/Templates'
    ));

    promises.push(copy.glob(
        filesDir + '/Templates/**/*.json',
        filesDir + '/Templates',
        outDir + '/edity/Templates'
    ));

    promises.push(less.compile({
        encoding: 'utf8',
        importPaths: [filesDir, moduleDir + '/bootstrap/less'],
        input: filesDir + '/Templates/**/*.less',
        basePath: filesDir + '/Templates',
        out: outDir + '/edity/Templates',
        compress: true,
    }));

    promises.push(copy.glob(filesDir + "/edity.json", filesDir, outDir + '/edity'));

    return Promise.all(promises);
}