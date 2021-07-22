import * as bootstrap from 'htmlrapier.bootstrap/src/main';
import { Fetcher } from 'htmlrapier/src/fetcher';
import { WindowFetch } from 'htmlrapier/src/windowfetch';
import { WithCredentialsFetcher } from 'editymceditface.client/EditorCore/WithCredentialsFetcher';
import * as urlInjector from 'editymceditface.client/EditorCore/BaseUrlInjector';
import * as controller from 'htmlrapier/src/controller';
import * as client from 'editymceditface.client/EditorCore/EdityHypermediaClient';
import * as pageConfig from 'htmlrapier/src/pageconfig';
import * as hr from 'htmlrapier/src/main';
import * as bootstrap4form from 'htmlrapier.form.bootstrap4/src/main';

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