using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.Page;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]/[action]")]
    public class PageController : Controller
    {
        private FileFinder fileFinder;

        public PageController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet("{*file}")]
        public PageSettings Settings(String file)
        {
            TargetFileInfo targetFile = new TargetFileInfo(file);
            var definition = fileFinder.getPageDefinition(targetFile);
            String title;
            if(!definition.Vars.TryGetValue("title", out title))
            {
                title = "Untitiled";
            }
            return new PageSettings()
            {
                Title = title
            };
        }

        [HttpPost("{*file}")]
        public void Settings(String file, [FromBody]PageSettings settings)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException();
            }

            TargetFileInfo targetFile = new TargetFileInfo(file);
            var definition = fileFinder.getPageDefinition(targetFile);
            definition.Vars["title"] = settings.Title;
            fileFinder.savePageDefinition(definition, targetFile);
        }
    }
}
