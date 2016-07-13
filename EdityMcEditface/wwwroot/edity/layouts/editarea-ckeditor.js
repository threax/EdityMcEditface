(function ($, h) {
    // config
    var editor = document.getElementById('editArea');

    CKEDITOR.editorConfig = function (config) {
        config.toolbarGroups = [
            { name: 'clipboard', groups: ['clipboard', 'undo'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
            { name: 'links', groups: ['links'] },
            { name: 'insert', groups: ['insert'] },
            { name: 'forms', groups: ['forms'] },
            { name: 'tools', groups: ['tools'] },
            { name: 'document', groups: ['mode', 'document', 'doctools'] },
            { name: 'others', groups: ['others'] },
            '/',
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
            { name: 'styles', groups: ['styles'] },
            { name: 'colors', groups: ['colors'] },
            { name: 'about', groups: ['about'] }
        ];

        config.removeButtons = 'Underline,Subscript,Superscript,Scayt,Maximize,Source,About,Add alert box';

        config.allowedContent = true;
        config.extraPlugins = 'colorbutton,youtube,uploadimage,widgetbootstrap';
        config.imageUploadUrl = '/edity/Page/Asset/' + window.location.pathname;
    };

    var sourceText = $('#source');

    $('#ApplySourceChanges').submit(function (event) {
        $('#sourceModal').modal('hide');
        return false;
    });

    var buttonCreation = h.storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SaveButton",
        created: function (component) {
            var jqComponent = $(component);

            var loading = $(component.first("SaveLoadbar"));
            loading.hide();

            component.bind({
                SaveButton: {
                    click: function (evt) {
                        evt.preventDefault();

                        loading.fadeIn(200);
                        var content = editor.innerHTML;
                        var blob = new Blob([content], { type: "text/html" });
                        h.rest.upload($(this).attr('href') + '/' + window.location.pathname, blob, function () {
                            loading.fadeOut(200);
                        },
                        function () {
                            loading.fadeOut(200);
                            alert("Error saving page. Please try again later.");
                        });
                    }
                }
            });
        }
    });

    buttonCreation.push({
        name: "PreviewButton"
    });

    buttonCreation.push({
        name: "EditSourceNavItem",
        created: function (button) {
            button.bind({
                EditSource: {
                    click: function () {
                        var content = editor.innerHTML;
                        sourceText.val(content);
                    }
                }
            });
        }
    });
})(jQuery, htmlrest);