import * as bootstrap from 'hr.bootstrap.all';
import { Fetcher } from 'hr.fetcher';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import { WithCredentialsFetcher } from 'edity.editorcore.WithCredentialsFetcher';
import * as urlInjector from 'edity.editorcore.BaseUrlInjector';
import * as controller from 'hr.controller';
import * as client from 'edity.editorcore.EdityHypermediaClient';

interface PageSettings {
    baseUrl;
}

var mainBuilder: controller.InjectedControllerBuilder = null;

export function createBaseBuilder(): controller.InjectedControllerBuilder {
    if (!mainBuilder) {
        mainBuilder = new controller.InjectedControllerBuilder();

        bootstrap.activate();

        var pageSettings = <PageSettings>(<any>window).editPageSettings;
        if (!pageSettings) {
            pageSettings = {
                baseUrl: '/'
            };
        }

        var services = mainBuilder.Services;
        services.addShared(Fetcher, s => new WithCredentialsFetcher(new CacheBuster(new WindowFetch())));
        services.addShared(urlInjector.IBaseUrlInjector, s => new urlInjector.BaseUrlInjector(pageSettings.baseUrl));
        services.addShared(client.EntryPointInjector, s => new client.EntryPointInjector(pageSettings.baseUrl ? pageSettings.baseUrl : '/' + 'edity/entrypoint', s.getRequiredService(Fetcher)));
    }
    return mainBuilder;
}