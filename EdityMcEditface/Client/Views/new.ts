"use strict";


import * as http from "hr.http";
import * as uploader from "clientlibs.uploader";
import * as component from "hr.components";
import * as domQuery from "hr.domquery";
import * as controller from "hr.controller";
import { init } from 'clientlibs.PageStart';
import { TemplateClient, Template } from "clientlibs.EdityClient";

function createPage(path, uploadPath) {
    http.get(path + ".html")
        .then(function (templateData) {
            //Make a blob
            var blob = new Blob([templateData], { type: "text/html" });
            return uploader.upload(uploadPath + window.location.pathname, blob);
        })
        .then(function (data) {
            window.location.href = window.location.href;
        })
        .catch(function (data) {
            alert('Could not create new page. Please try again later');
        });
}

class TemplateItemController {
    private context;
    private data;

    constructor(bindings, context, data) {
        this.data = data;
        this.context = context;
    }

    create(evt) {
        evt.preventDefault();
        createPage(this.data.path, this.context.createpath);
    }
}

class NewPageController {
    constructor(bindings: controller.BindingCollection, context: TemplateClient) {
        var templatesModel = bindings.getModel<Template[]>('templates');
        var config = bindings.getConfig();

        context.listAll(null)
            .then((data) => {
                templatesModel.setData(data, controller.createOnCallback<TemplateItemController, any, void>(TemplateItemController, config));
            })
            .catch((err) => {
                alert('Cannot load templates, please try again later');
            });
    }
}


init().then((pageConfig) => {
    var client = new TemplateClient(pageConfig.BaseUrl, pageConfig.Fetcher);
    controller.create<NewPageController, TemplateClient, void>("new", NewPageController, client);
});