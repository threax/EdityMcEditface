"use strict";

jsns.run(function (using) {
    var rest = using("htmlrest.rest");
    var component = using("htmlrest.components");
    var domQuery = using("htmlrest.domquery");

    rest.get('edity/templates', function (data) {
        //This should be sanitized, also check for server error
        //TODO: use a component
        var templateList = domQuery.first('[data-template-list]');
        component.repeat("new-template-preview", templateList, data, function (element, data) {
            element.setListener({
                create: function (evt) {
                    evt.preventDefault();

                    rest.get(this.getAttribute('data-template') + ".html", function (templateData) {
                        //Make a blob
                        var blob = new Blob([templateData], { type: "text/html" });
                        rest.upload(templateList.getAttribute('data-template-list') + window.location.pathname + ".html", blob, function () {
                            window.location.href = window.location.href + ".html";
                        });
                    });
                }
            });
        });
    });
});