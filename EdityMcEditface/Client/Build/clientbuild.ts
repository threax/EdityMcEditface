"use strict";
var copy = require('threax-npm-tk/copy');
var less = require('threax-npm-tk/less');

module.exports = async function (outDir, iconOutPath, moduleDir) {
    await less({
        encoding: 'utf8',
        importPaths: [__dirname, moduleDir + '/bootstrap/less'],
        input: __dirname + '/edity/**/*.less',
        basePath: __dirname + '/edity',
        out: outDir + '/edity',
        compress: true,
    });

    await less({
        encoding: 'utf8',
        importPaths: [__dirname, moduleDir + '/bootstrap/less'],
        input: __dirname + '/Views/**/*.less',
        basePath: __dirname + '/Views',
        out: outDir + '/edity/layouts',
        compress: true,
    });

    //Views
    await copy.glob(
        __dirname + '/Views/**/*.css',
        __dirname + '/Views',
        outDir + '/edity/layouts'
    );

    await copy.glob(
        __dirname + '/Views/**/*.html',
        __dirname + '/Views',
        outDir + '/edity/layouts'
    );

    //Templates
    await copy.glob(
        __dirname + '/Templates/**/*.css',
        __dirname + '/Templates',
        outDir + '/edity/Templates'
    );

    await copy.glob(
        __dirname + '/Templates/**/*.html',
        __dirname + '/Templates',
        outDir + '/edity/Templates'
    );
}