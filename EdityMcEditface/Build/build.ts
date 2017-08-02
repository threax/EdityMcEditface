import * as copy from 'threax-npm-tk/copy';
import * as less from 'threax-npm-tk/less';
import { tsc } from 'threax-npm-tk/typescript';
import * as jsnsTools from 'threax-npm-tk/jsnstools';
import * as artifact from 'threax-npm-tk/artifacts';

var filesDir = __dirname + "/..";

(async function () {
    try {
        await build(filesDir + "/ClientBin/Site", filesDir + "/wwwroot", filesDir + "/node_modules");
        console.log("Build SUCCEEDED");
    }
    catch (err) {
        console.log("Build FAILED: " + err.message);
    }
})();

export function build(outDir, iconOutPath, moduleDir): Promise<any> {
    var promises = [];

    promises.push(compileTypescript());

    promises.push(artifact.importConfigs(filesDir, filesDir + "/ClientBin/EdityMcEditface", ["node_modules/*/edity-artifacts.json"]));
    promises.push(artifact.importConfigs(filesDir, filesDir + "/ClientBin/Site", [filesDir + '/artifacts.json', artifact.getDefaultGlob(filesDir)]));

    //Return composite promise
    return Promise.all(promises);
}

async function compileTypescript() {
    await tsc({
        projectFolder: filesDir
    });

    await jsnsTools.saveLoadedModules(filesDir + '/wwwroot/lib/tsbin.js', ['hr.runattributes', 'edity.theme.layouts.default'], filesDir + '/ClientBin/Site/lib/tsbin.prod.js');
}