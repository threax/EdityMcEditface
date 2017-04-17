"use strict";
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');

module.exports = function (outDir, iconOutPath, moduleDir) {
    var promises = [];

    promises.push(less({
        encoding: 'utf8',
        importPaths: [__dirname, moduleDir + '/bootstrap/less'],
        input: __dirname + '/edity/**/*.less',
        basePath: __dirname + '/edity',
        out: outDir + '/edity',
        compress: true,
    }));

    promises.push(less({
        encoding: 'utf8',
        importPaths: [__dirname, moduleDir + '/bootstrap/less'],
        input: __dirname + '/Views/**/*.less',
        basePath: __dirname + '/Views',
        out: outDir + '/edity/layouts',
        compress: true,
    }));

    //Views
    promises.push(copy.glob(
        __dirname + '/Views/**/*.css',
        __dirname + '/Views',
        outDir + '/edity/layouts'
    ));

    promises.push(copy.glob(
        __dirname + '/Views/**/*.html',
        __dirname + '/Views',
        outDir + '/edity/layouts'
    ));

    //Templates
    promises.push(copy.glob(
        __dirname + '/Templates/**/*.css',
        __dirname + '/Templates',
        outDir + '/edity/Templates'
    ));

    promises.push(copy.glob(
        __dirname + '/Templates/**/*.html',
        __dirname + '/Templates',
        outDir + '/edity/Templates'
    ));

    return Promise.all(promises);
}