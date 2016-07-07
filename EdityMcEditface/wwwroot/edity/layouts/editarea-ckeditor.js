(function ($, h) {
    var loading = $(".load-linebar");

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

        config.removeButtons = 'Underline,Subscript,Superscript,Scayt,Maximize,Source,About';

        config.allowedContent = true;
        config.extraPlugins = 'colorbutton,youtube';
    };

    var sourceText = $('#source');

    $('#EditSource').click(function () {
        var content = editor.innerHTML;
        sourceText.val(content);
    });

    $('[data-edit-save]').click(function () {
        loading.fadeIn(200);
        var content = editor.innerHTML;
        var blob = new Blob([content], { type: "text/html" });
        h.rest.upload($(this).attr('href') + '/' + window.location.pathname, blob, function () {
            loading.fadeOut(200);
        }, true);
        return false;
    });

    $('#ApplySourceChanges').submit(function (event) {
        //pen.destroy();
        //pen.setContent(sourceText.val());
        //pen.rebuild();
        $('#sourceModal').modal('hide');
        return false;
    });
})(jQuery, htmlrest);