"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.bindingcollection"
],
function(exports, module, storage, rest, BindingCollection) {

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

    var modal = new BindingCollection('#sourceModal');
    modal.setListener({
        apply: function (evt) {
            evt.preventDefault();
            $('#sourceModal').modal('hide');
        }
    });

    var sourceModel = modal.getModel('source');

    var editSourceDialog = modal.getToggle('dialog');

    var buttonCreation = storage.getInInstance("edit-nav-menu-items", []);
    buttonCreation.push({
        name: "SaveButton",
        created: function (component) {
            var load = component.getToggle("load");
            load.off();

            component.setListener({
                save: function (evt) {
                    evt.preventDefault();

                    load.on();
                    var content = editor.innerHTML;
                    var blob = new Blob([content], { type: "text/html" });
                    rest.upload(this.getAttribute('href') + '/' + window.location.pathname, blob, function () {
                        load.off();
                    },
                    function () {
                        load.off();
                        alert("Error saving page. Please try again later.");
                    });
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
            button.setListener({
                edit: function () {
                    editSourceDialog.on();
                    sourceModel.setData({
                        source: editor.innerHTML
                    });
                }
            });
        }
    });
});