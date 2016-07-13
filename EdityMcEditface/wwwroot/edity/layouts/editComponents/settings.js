(function($, h){
    var settingsLifecycle = new h.form.ajaxLifecycle({
        formQuery: '#SettingsForm',
        loadingDisplayQuery: '#SettingsLoading',
        mainDisplayQuery: '#SettingsForm',
        populateFailDisplayQuery: '#SettingsLoadFailed'
    });

    $('#LoadSettingsAgainButton').click(function () {
        settingsLifecycle.populateData();
    });

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SettingsNavItem",
        created: function (button) {
            h.component.bind(button, {
                SettingsButton: {
                    click: function(){
                        settingsLifecycle.populateData();
                    }
                }
            });
        }
    });
})(jQuery, htmlrest);