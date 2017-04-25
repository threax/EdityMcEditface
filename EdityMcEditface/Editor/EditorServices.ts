import * as bootstrap from 'hr.bootstrap.all';
import { AccessTokenManager } from 'hr.accesstokens';
import { Fetcher } from 'hr.fetcher';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import { WithCredentialsFetcher } from 'edity.editorcore.WithCredentialsFetcher';
import { CompilerService } from 'edity.editorcore.CompileService';
import * as di from 'hr.di';
import * as urlInjector from 'edity.editorcore.BaseUrlInjector';

interface PageSettings {
    baseUrl;
}

export function addServices(services: di.ServiceCollection) {
    bootstrap.activate();

    var pageSettings = <PageSettings>(<any>window).editPageSettings;
    if (!pageSettings) {
        pageSettings = {
            baseUrl: '/'
        };
    }

    services.tryAddShared(Fetcher, s => new WithCredentialsFetcher(new CacheBuster(new WindowFetch())));
    services.tryAddShared(urlInjector.IBaseUrlInjector, s => new urlInjector.BaseUrlInjector(pageSettings.baseUrl));
}