(function ($, h)
{
    var templates = $('[data-template-list]');
    h.rest.get('edity/templates', function (data)
    {
        //This should be sanitized, also check for server error
        //TODO: use a component
        h.component.repeat("new-template-preview", templates, data, function (element, data) {
            element.bind('click.new', function ()
            {
                var sender = $(this);
                h.rest.get(sender.attr('data-template') + ".html", function (templateData)
                {
                    //Make a blob
                    var blob = new Blob([templateData], { type: "text/html" });
                    h.rest.upload(templates.attr('data-template-list') + window.location.pathname + ".html", blob, function ()
                    {
                        window.location.href = window.location.href + ".html";
                    });
                });
                
                return false;
            });
        });
    });
})(jQuery, htmlrest)