"use strict";

jsns.run([
    "htmlrest.rest",
    "htmlrest.components",
    "htmlrest.domquery",
    "htmlrest.controller"
],
function (exports, module, rest, component, domQuery, controller) {

    function createPage(path, uploadPath) {
        rest.get(path + ".html", function (templateData) {
            //Make a blob
            var blob = new Blob([templateData], { type: "text/html" });
            rest.upload(uploadPath + window.location.pathname + ".html", blob, function () {
                window.location.href = window.location.href + ".html";
            },
            function () {
                alert('Could not create new page. Please try again later');
            });
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
        rest.get(templatesModel.getSrc(),
            function (data) {
                templatesModel.setData(data, controller.createOnCallback(TemplateItemController, config));
            },
            function (data) {
                alert('Cannot load templates, please try again later');
            });
    }

    controller.create("new", NewController);
});