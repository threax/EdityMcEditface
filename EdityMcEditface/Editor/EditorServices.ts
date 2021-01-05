import * as bootstrap from 'hr.bootstrap.main';
import { Fetcher } from 'hr.fetcher';
import { WindowFetch } from 'hr.windowfetch';
import { WithCredentialsFetcher } from 'edity.editorcore.WithCredentialsFetcher';
import * as urlInjector from 'edity.editorcore.BaseUrlInjector';
import * as controller from 'hr.controller';
import * as client from 'edity.editorcore.EdityHypermediaClient';
import * as pageConfig from 'hr.pageconfig';
import * as hr from 'hr.main';
import * as bootstrap4form from 'hr.form.bootstrap4.main';

//Activate htmlrapier
hr.setup();
bootstrap.setup();
bootstrap4form.setup();

interface Config {
    editSettings?: {
        baseUrl: string;
    };
}

let mainBuilder: controller.InjectedControllerBuilder = null;

export function createBaseBuilder(): controller.InjectedControllerBuilder {
    if (!mainBuilder) {
        mainBuilder = new controller.InjectedControllerBuilder();

        let config = pageConfig.read<Config>();
        if (!config) {
            config = {};
        }
        if (!config.editSettings) {
            config.editSettings = {
                baseUrl: '/'
            };
        }

        const services = mainBuilder.Services;
        services.addShared(Fetcher, s => new WithCredentialsFetcher(new WindowFetch()));
        services.addShared(urlInjector.IBaseUrlInjector, s => new urlInjector.BaseUrlInjector(config.editSettings.baseUrl));
        const baseUrl = config.editSettings.baseUrl ? config.editSettings.baseUrl : '';
        mainBuilder.Services.addShared(client.EntryPointInjector, s => new client.EntryPointInjector(baseUrl + '/edity/entrypoint', s.getRequiredService(Fetcher)));
    }
    return mainBuilder;
}