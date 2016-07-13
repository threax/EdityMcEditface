(function ($, h) {
    function CompilerMessages() {
        var startMessage = $('[data-compile-message-startup]');
        var successMessage = $('[data-compile-message-success]');
        var failMessage = $('[data-compile-message-fail]');
        var compilingMessage = $('[data-compile-compiling]');
        var self = this;

        this.hideAll = function () {
            startMessage.hide();
            successMessage.hide();
            failMessage.hide();
            compilingMessage.hide();
        }

        this.starting = function () {
            self.hideAll();
            startMessage.show();
        }

        this.compiling = function () {
            self.hideAll();
            compilingMessage.show();
        }

        this.succeeded = function () {
            self.hideAll();
            successMessage.show();
        }

        this.failed = function () {
            self.hideAll();
            failMessage.show();
        }
    };

    var compilerOutputMessage = new CompilerMessages();

    var compileButton = $('#CompileButton');

    compileButton.click(function () {
        compileButton.prop('disabled', true);
        compilerOutputMessage.compiling();
        h.rest.post('/edity/Compile', {},
        function () {
            compileButton.prop('disabled', false);
            compilerOutputMessage.succeeded();
        }, function () {
            compileButton.prop('disabled', false);
            compilerOutputMessage.failed();
        });
        return false;
    });

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "CompileNavItem",
        created: function (button) {
            h.component.bind(button, {
                CompileModalButton: {
                    click: function () {
                        compilerOutputMessage.starting();
                    }
                }
            });
        }});
})(jQuery, htmlrest);