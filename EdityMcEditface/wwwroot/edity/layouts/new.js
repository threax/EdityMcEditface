﻿"use strict";

jsns.run(function (using) {
    var rest = using("htmlrest.rest");
    var component = using("htmlrest.components");
    var domQuery = using("htmlrest.domquery");
    var BindingCollection = using("htmlrest.bindingcollection").BindingCollection;

    var templateBindings = new BindingCollection('#templates');

    rest.get('edity/templates', function (data) {
        var templatesModel = templateBindings.getModel('templates');
        templatesModel.setData(data, function (created, rowData) {
            created.setListener({
                create: function (evt) {
                    evt.preventDefault();

                    rest.get(this.getAttribute('data-template') + ".html", function (templateData) {
                        //Make a blob
                        var blob = new Blob([templateData], { type: "text/html" });
                        rest.upload(templatesModel.getSrc() + window.location.pathname + ".html", blob, function () {
                            window.location.href = window.location.href + ".html";
                        },
                        function () {
                            alert('Could not create new page. Please try again later');
                        });
                    });
                }
            });
        });
    },
    function (data) {
        alert('Cannot load templates, please try again later');
    });
});