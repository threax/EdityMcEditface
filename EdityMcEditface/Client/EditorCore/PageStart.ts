import * as bootstrap from 'hr.bootstrap.all';
import { AccessTokenManager } from 'hr.accesstokens';
import { Fetcher } from 'hr.fetcher';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import { WithCredentialsFetcher } from 'edity.editorcore.WithCredentialsFetcher';
import { TreeMenuController } from 'edity.widgets.TreeMenu';
//controller.create<TreeMenuController, void, void>("treeMenu", TreeMenuController);

export class PageStart {
    //Configuration
    private fetcher: Fetcher;

    //Page Start
    constructor() {
        bootstrap.activate();
        this.fetcher = new WithCredentialsFetcher(new CacheBuster(new WindowFetch()));
    }

    get Fetcher(): Fetcher {
        return this.fetcher;
    }

    get BaseUrl(): string {
        return "";
    }
}

var instance: PageStart = null;
/**
 * Set up common config for the page to run.
 * @returns
 */
export function init(): Promise<PageStart> {
    if (instance === null) {
        instance = new PageStart();
        TreeMenuController.GetBuilder().create("treeMenu");
    }

    return Promise.resolve(instance);
}