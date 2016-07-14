(function ($, h) {
    var settingsBindings = new h.component.BindingCollection('#settingsModal');

    var settingsLifecycle = new h.form.ajaxLifecycle(settingsBindings);

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SettingsNavItem",
        created: function (button) {
            button.bind({
                SettingsButton: {
                    click: function(){
                        settingsLifecycle.populateData();
                    }
                }
            });
        }
    });
})(jQuery, htmlrest);