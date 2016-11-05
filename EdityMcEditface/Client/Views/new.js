"use strict";

jsns.run([
    "hr.http",
    "hr.components",
    "hr.domquery",
    "hr.controller"
],
function (exports, module, http, component, domQuery, controller) {

    function createPage(path, uploadPath) {
        http.get(path + ".html")
        .then(function (templateData) {
            //Make a blob
            var blob = new Blob([templateData], { type: "text/html" });
            return http.upload(uploadPath + window.location.pathname, blob);
        })
        .then(function (data) {
            window.location.href = window.location.href;
        })
        .catch(function (data) {
            alert('Could not create new page. Please try again later');
        });
    }

    function TemplateItemController(bindings, context, data) {
        function create(evt) {
            evt.preventDefault();
            createPage(data.path, context.createpath);
        }
        this.create = create;
    }

    function NewController(bindings) {
        var templatesModel = bindings.getModel('templates');
        var config = bindings.getConfig();
        http.get(templatesModel.getSrc())
        .then(function (data) {
            templatesModel.setData(data, controller.createOnCallback(TemplateItemController, config));
        })
        .catch(function (data) {
            alert('Cannot load templates, please try again later');
        });
    }

    controller.create("new", NewController);
});