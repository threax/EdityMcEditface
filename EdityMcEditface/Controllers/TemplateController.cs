using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    public class TemplateController : Controller
    {
        private FileFinder fileFinder;

        public TemplateController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet("edity/templates")]
        public IActionResult Index()
        {
            return Json(from q in Directory.EnumerateFiles(fileFinder.getFullRealPath("edity/templates"))
                   select new Template()
                   {
                       Path = Path.ChangeExtension(fileFinder.getUrlFromSystemPath(q), null),
                   });

        }
    }
}
