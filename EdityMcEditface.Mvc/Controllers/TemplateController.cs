using EdityMcEditface.HtmlRenderer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This api controls templates.
    /// </summary>
    [Authorize(Roles = Roles.EditPages)]
    [Route("edity/[controller]/[action]")]
    public class TemplateController : Controller
    {
        private IFileFinder fileFinder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileFinder"></param>
        public TemplateController(IFileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// Get all the templates in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Template> ListAll()
        {
            return fileFinder.Templates;

        }

        /// <summary>
        /// Get a single template's content.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("text/html")]
        public FileStreamResult GetContent([FromQuery] String file)
        {
            var template = fileFinder.Templates.First(i => i.Path == file);
            return returnFile(Path.ChangeExtension(template.Path, "html"));
        }

        private FileStreamResult returnFile(String file)
        {
            var content = new FileExtensionContentTypeProvider();
            String contentType;
            if (content.TryGetContentType(file, out contentType))
            {
                return new FileStreamResult(fileFinder.ReadFile(file), contentType);
            }
            throw new FileNotFoundException($"Cannot find file type for '{file}'", file);
        }
    }
}
