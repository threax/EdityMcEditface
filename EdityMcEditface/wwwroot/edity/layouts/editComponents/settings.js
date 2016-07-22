"use strict";

jsns.run(function (using) {
    var BindingCollection = using("htmlrest.bindingcollection");
    var FormLifecycle = using("htmlrest.formlifecycle");
    var storage = using("htmlrest.storage");
    var toggles = using("htmlrest.toggles");
    var rest = using("htmlrest.rest");

    var bindings = new BindingCollection('#settingsModal');
    var tryAgainFunc = populate;

    bindings.setListener({
        submit: function (evt) {
            evt.preventDefault();
            submit();
        },
        tryAgain: function (evt) {
            evt.preventDefault();
            tryAgainFunc();
        }
    });

    var load = bindings.getToggle('load');
    var main = bindings.getToggle('main');
    var fail = bindings.getToggle('fail');
    var formToggler = new toggles.group(load, main, fail);

    var settingsModel = bindings.getModel('settings');

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SettingsNavItem",
        created: function (button) {
            button.setListener({
                open: function () {
                    populate();
                }
            });
        }
    });

    function populate() {
        formToggler.show(load);
        rest.get(settingsModel.getSrc(),
            function (successData) {
                settingsModel.setData(successData);
                formToggler.show(main);
            },
            function (failData) {
                tryAgainFunc = populate;
                formToggler.show(fail);
            });
    }

    function submit() {
        formToggler.show(load);
        var data = settingsModel.getData();
        rest.post(settingsModel.getSrc(), data,
            function (successData) {
                formToggler.show(main);
            },
            function (failData) {
                tryAgainFunc = submit;
                formToggler.show(fail);
            });
    }
});