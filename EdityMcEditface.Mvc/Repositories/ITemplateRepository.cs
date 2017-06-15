using System.Collections.Generic;
using System.IO;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Templates;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface ITemplateRepository
    {
        /// <summary>
        /// Get a single template's content.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Stream GetContent(string file);

        /// <summary>
        /// Get all the templates in the system.
        /// </summary>
        /// <returns></returns>
        TemplateCollection ListAll();

        /// <summary>
        /// Find the template specified by file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Template GetTemplate(string file);
    }
}