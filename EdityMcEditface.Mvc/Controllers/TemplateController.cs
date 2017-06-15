using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Models.Templates;
using EdityMcEditface.Mvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This api controls templates.
    /// </summary>
    [Authorize(Roles = Roles.EditPages)]
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    public class TemplateController : Controller
    {
        private ITemplateRepository templateRepo;

        public static class Rels
        {
            public const String List = "ListTemplates";
            public const String GetContent = "GetContent";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateRepo"></param>
        public TemplateController(ITemplateRepository templateRepo)
        {
            this.templateRepo = templateRepo;
        }

        /// <summary>
        /// Get all the templates in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public TemplateCollection ListAll()
        {
            return templateRepo.ListAll();
        }

        /// <summary>
        /// Get a single template's content.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("Content/{*Path}")]
        [Produces("text/html")]
        [HalRel(Rels.GetContent)]
        public FileStreamResult GetContent(String path)
        {
            var template = templateRepo.GetTemplate(path);
            var htmlFile = Path.ChangeExtension(template.Path, "html");
            return returnFile(htmlFile, templateRepo.GetContent(template.Path)); //This stream will be closed by the result, do not need to using it
        }

        private FileStreamResult returnFile(String file, Stream stream)
        {
            var content = new FileExtensionContentTypeProvider();
            String contentType;
            if (content.TryGetContentType(file, out contentType))
            {
                return new FileStreamResult(stream, contentType);
            }
            throw new FileNotFoundException($"Cannot find file type for '{file}'", file);
        }
    }
}
