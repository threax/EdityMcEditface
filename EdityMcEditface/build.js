var clientBuild = require('./Client/build');
var tsc = require('threax-npm-tk/tsc');
var jsonEditorBuild = require('htmlrapier.json-editor/build');

jsonEditorBuild(__dirname + "/wwwroot/lib");
clientBuild(__dirname + "/wwwroot/lib", __dirname + "/wwwroot");
tsc();