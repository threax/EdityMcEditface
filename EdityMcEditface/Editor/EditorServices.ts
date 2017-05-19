import * as bootstrap from 'hr.bootstrap.all';
import { Fetcher } from 'hr.fetcher';
import { CacheBuster } from 'hr.cachebuster';
import { WindowFetch } from 'hr.windowfetch';
import { WithCredentialsFetcher } from 'edity.editorcore.WithCredentialsFetcher';
import * as urlInjector from 'edity.editorcore.BaseUrlInjector';
import * as controller from 'hr.controller';

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
        services.tryAddShared(Fetcher, s => new WithCredentialsFetcher(new CacheBuster(new WindowFetch())));
        services.tryAddShared(urlInjector.IBaseUrlInjector, s => new urlInjector.BaseUrlInjector(pageSettings.baseUrl));
    }
    return mainBuilder;
}