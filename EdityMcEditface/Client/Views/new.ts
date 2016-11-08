"use strict";

import * as http from "hr.http";
import * as uploader from "clientlibs.uploader";
import * as component from "hr.components";
import * as domQuery from "hr.domquery";
import * as controller from "hr.controller";
import { init, PageStart } from 'clientlibs.PageStart';
import { TemplateClient, Template, PageClient } from "clientlibs.EdityClient";

interface TemplateItemControllerSettings {
    config: NewPageControllerConfig;
    pageConfig: PageStart,
    templateClient: TemplateClient
    pageClient: PageClient
}

class TemplateItemController {
    static GetCreator(context: TemplateItemControllerSettings): controller.ControllerBuilder<TemplateItemController, TemplateItemControllerSettings, Template> {
        return new controller.ControllerBuilder<TemplateItemController, TemplateItemControllerSettings, Template>(TemplateItemController, context);
    }

    private context: TemplateItemControllerSettings;
    private data: Template;

    constructor(bindings: controller.BindingCollection, context: TemplateItemControllerSettings, data: Template) {
        this.data = data;
        this.context = context;
    }

    create(evt) {
        evt.preventDefault();
        this.context.templateClient.getContent(this.data.path, null)
            .then((templateData) => {
                //Make a blob
                var blob = new Blob([templateData], { type: "text/html" });
                return this.context.pageClient.save(window.location.pathname, { data: blob, fileName: 'file.html' }, null);
            })
            .then(function (data) {
                window.location.href = window.location.href;
            })
            .catch(function (data) {
                alert('Could not create new page. Please try again later');
            });

    }
}

interface NewPageControllerConfig {
    createpath: string;
}

class NewPageController {
    constructor(bindings: controller.BindingCollection, context: PageStart) {
        var templatesModel = bindings.getModel<Template[]>('templates');
        var config = bindings.getConfig<NewPageControllerConfig>();
        var templateClient = new TemplateClient(context.BaseUrl, context.Fetcher);
        var pageClient = new PageClient(context.BaseUrl, context.Fetcher);

        templateClient.listAll(null)
            .then((data) => {
                templatesModel.setData(data, TemplateItemController.GetCreator({
                    config: config,
                    pageConfig: context,
                    templateClient: templateClient,
                    pageClient: pageClient
                }).createOnCallback());
            })
            .catch((err) => {
                alert('Cannot load templates, please try again later');
            });
    }
}


init().then((pageConfig) => {
    controller.create<NewPageController, PageStart, void>("new", NewPageController, pageConfig);
});