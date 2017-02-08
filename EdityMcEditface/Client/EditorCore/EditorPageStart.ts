import * as bootstrap from 'hr.bootstrap.all';
import { AccessTokenManager } from 'hr.accesstokens';
import { Fetcher } from 'hr.fetcher';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import { WithCredentialsFetcher } from 'edity.editorcore.WithCredentialsFetcher';

interface PageSettings {
    baseUrl;
}

export class EditorPageStart {
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
        var pageSettings = <PageSettings>(<any>window).editPageSettings;
        if (pageSettings) {
            return pageSettings.baseUrl;
        }
        return "";
    }
}

var instance: EditorPageStart = null;
/**
 * Set up common config for the page to run.
 * @returns
 */
export function init(): Promise<EditorPageStart> {
    if (instance === null) {
        instance = new EditorPageStart();
    }

    return Promise.resolve(instance);
}