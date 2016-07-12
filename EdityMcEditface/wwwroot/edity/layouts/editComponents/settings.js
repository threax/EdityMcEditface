(function($, h){
    var settingsLifecycle = new h.form.ajaxLifecycle({
        formQuery: '#SettingsForm',
        loadingDisplayQuery: '#SettingsLoading',
        mainDisplayQuery: '#SettingsForm',
        populateFailDisplayQuery: '#SettingsLoadFailed'
    });

    $('#SettingsButton').click(function () {
        settingsLifecycle.populateData();
    });

    $('#LoadSettingsAgainButton').click(function () {
        settingsLifecycle.populateData();
    });
})(jQuery, htmlrest);