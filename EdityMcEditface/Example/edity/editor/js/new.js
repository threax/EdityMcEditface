(function ($, h)
{
    var templates = $('#TemplateList');
    h.run(templates, function ()
    {
        return [
            h.rest.get('/ide/api/dir/edity/templates/'),
            h.func(function (data)
            {
                //This should be sanitized
                var templateList = '';
                data.data.files.forEach(function (i)
                {
                    templateList += '<li><a data-template="' + i + '">' + i + '</a></li>';
                });
                templates.html(templateList);

                $('[data-template]').off('click.new').on('click.new', function ()
                {

                });
            })
        ]
    });
})(jQuery, htmlrest)