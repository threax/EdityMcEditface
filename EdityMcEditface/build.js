var clientBuild = require('./Client/build');
var tsc = require('threax-npm-tk/tsc');

clientBuild(__dirname + "/ClientBin/EdityMcEditface", __dirname + "/wwwroot", __dirname + "/node_modules");
tsc();