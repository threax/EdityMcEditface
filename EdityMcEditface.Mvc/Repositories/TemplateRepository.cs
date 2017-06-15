using AutoMapper;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EdityMcEditface.Mvc.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private IFileFinder fileFinder;
        private IMapper mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileFinder">The file finder.</param>
        /// <param name="mapper">The automapper instance.</param>
        public TemplateRepository(IFileFinder fileFinder, IMapper mapper)
        {
            this.fileFinder = fileFinder;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get all the templates in the system.
        /// </summary>
        /// <returns></returns>
        public TemplateCollection ListAll()
        {
            return new TemplateCollection(fileFinder.Templates.Select(i => mapper.Map<TemplateView>(i)));
        }

        /// <summary>
        /// Find the template specified by file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Template GetTemplate(String file)
        {
            var normalizedFile = Path.GetFullPath(file.EnsureStartingPathSlash());
            return fileFinder.Templates.First(i => Path.GetFullPath(i.Path) == normalizedFile);
        }

        /// <summary>
        /// Get a single template's content.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Stream GetContent(String file)
        {
            var template = GetTemplate(file);
            var htmlFile = Path.ChangeExtension(template.Path, "html");
            return fileFinder.ReadFile(htmlFile);
        }
    }
}
