(function ($, h)
{
    var templates = $('#TemplateList');
    var data = h.rest.get('/edity/list/edity/templates/', function (data)
    {
        //This should be sanitized, also check for server error
        //TODO: use a component
        var templateList = '';
        data.files.forEach(function (i)
        {
            templateList += '<li><a href="#" data-template="/' + i + '">' + i + '</a></li>';
        });
        templates.html(templateList);

        var templateItems = $('[data-template]');
        templateItems.unbind('click.new');
        templateItems.bind('click.new', function ()
        {
            var boundry = "blob";
            var sender = $(this);
            h.rest.get(sender.attr('data-template'), function (templateData)
            {
                //Make a blob
                var blob = new Blob([templateData], { type: "text/html" });
                h.rest.upload(window.location.pathname + ".html", blob, function ()
                {
                    window.location.href = window.location.href + ".html";
                });
            });

            return false;
        });
    });
})(jQuery, htmlrest)