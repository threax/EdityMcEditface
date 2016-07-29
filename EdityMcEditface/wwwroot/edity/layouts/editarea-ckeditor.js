"use strict";

jsns.run([
    "htmlrest.storage",
    "htmlrest.rest",
    "htmlrest.controller",
    "htmlrest.widgets.navmenu"
],
function (exports, module, storage, rest, controller, navmenu) {

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

    function SaveController(component) {
        var load = component.getToggle("load");
        load.off();

        function save(evt) {
            var saveModel = component.getModel("save");

            evt.preventDefault();

            load.on();
            var content = editor.innerHTML;
            var blob = new Blob([content], { type: "text/html" });
            rest.upload(saveModel.getSrc() + '/' + window.location.pathname, blob, function () {
                load.off();
            },
            function () {
                load.off();
                alert("Error saving page. Please try again later.");
            });
        }
        this.save = save;
    }

    var editMenu = navmenu.getNavMenu("edit-nav-menu-items");
    editMenu.add("SaveButton", SaveController);
    editMenu.add("PreviewButton");

    function EditSourceController(bindings) {
        var sourceModel = bindings.getModel('source');
        var editSourceDialog = bindings.getToggle('dialog');

        function apply(evt) {
            evt.preventDefault();
            editSourceDialog.off();
            editor.innerHTML = sourceModel.getData().source;
        }
        this.apply = apply;

        function NavItemController() {
            function edit() {
                editSourceDialog.on();
                sourceModel.setData({
                    source: editor.innerHTML
                });
            }
            this.edit = edit;
        }

        editMenu.add("EditSourceNavItem", NavItemController);
    }

    controller.create("editSource", EditSourceController);
});