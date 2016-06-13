(function ($, h) {
    // config
    var options = {
        editor: document.getElementById('editArea'),
        list: [
          'insertimage', 'blockquote', 'h1', 'h2', 'h3', 'p', 'code', 'insertorderedlist', 'insertunorderedlist', 'inserthorizontalrule',
          'indent', 'outdent', 'bold', 'italic', 'underline', 'createlink'
        ],
        class: 'nopenstyle',
        stay: false,
        cleanAttrs: []
    };

    var loading = $(".load-linebar");

    // create editor
    var pen = window.pen = new Pen(options);

    pen.focus();

    var sourceText = $('#source');

    $('#EditSource').click(function () {
        var penContent = pen.getContent();
        sourceText.val(penContent);
    });

    $('[data-edit-save]').click(h.event(function () {
        return [
            h.func(function ()
            {
                loading.fadeIn(200);
                var blob = new Blob([pen.getContent()], { type: "text/html" });
                return blob;
            }),
            //h.func(function (data) {
            //    return data.data;
            //}),
            h.rest.upload(window.location.pathname),
            h.func(function ()
            {
                loading.fadeOut(200);
            })
        ];
    }));

    $('#ApplySourceChanges').submit(function (event) {
        pen.destroy();
        pen.setContent(sourceText.val());
        pen.rebuild();
        $('#sourceModal').modal('hide');
        return false;
    });

    // toggle editor mode
    //document.querySelector('#mode').addEventListener('click', function ()
    //{
    //    var text = this.textContent;
    //
    //    if (this.classList.contains('disabled'))
    //    {
    //        this.classList.remove('disabled');
    //        pen.rebuild();
    //    } else
    //    {
    //        this.classList.add('disabled');
    //        pen.destroy();
    //    }
    //});

    // export content as markdown
    //document.querySelector('#tomd').addEventListener('click', function ()
    //{
    //    var text = pen.toMd();
    //    document.body.innerHTML = '<a href="javascript:location.reload()">&larr;back to editor</a><br><br><pre>' + text + '</pre>';
    //});

    // toggle editor mode
    //document.querySelector('#hinted').addEventListener('click', function ()
    //{
    //    var pen = document.querySelector('.pen')
    //
    //    if (pen.classList.contains('hinted'))
    //    {
    //        pen.classList.remove('hinted');
    //        this.classList.add('disabled');
    //    } else
    //    {
    //        pen.classList.add('hinted');
    //        this.classList.remove('disabled');
    //    }
    //});
})(jQuery, htmlrest);