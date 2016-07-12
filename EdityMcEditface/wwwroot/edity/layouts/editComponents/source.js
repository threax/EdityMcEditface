(function ($, h) {
    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "EditSourceNavItem",
        created: function (button) { }
    });
})(jQuery, htmlrest);