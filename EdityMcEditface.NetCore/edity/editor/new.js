(function ($, h)
{
    var templates = $('#TemplateList');
    h.run(templates, function ()
    {
        return [
            h.rest.get('/ide/api/dir/edity/templates/'),
            h.func(function (data)
            {
                //This should be sanitized, also check for server error
                var templateList = '';
                data.data.files.forEach(function (i)
                {
                    templateList += '<li><a href="#" data-template="/' + i + '">' + i + '</a></li>';
                });
                templates.html(templateList);

                var templateItems = $('[data-template]');
                templateItems.unbind('click.new');
                templateItems.bind('click.new', h.event(function (sender) {
                    var boundry = "blob";
                    return [
                        h.rest.get(sender.attr('data-template')),
                        h.func(function (data) {
                            return data.data;
                        }),
                        h.rest.upload('/ide/api/upload' + window.location.pathname + ".html"),
                        h.func(function (data) {
                            window.location.href = window.location.href + ".html";
                        })
                    ];
                }));
            })
        ]
    });
})(jQuery, htmlrest)