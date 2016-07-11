﻿using EdityMcEditface.HtmlRenderer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Authorize(Roles = Roles.EditPages)]
    public class TemplateController : Controller
    {
        private FileFinder fileFinder;

        public TemplateController(FileFinder fileFinder)
        {
            this.fileFinder = fileFinder;
        }

        [HttpGet("edity/templates")]
        public IEnumerable<Template> Index()
        {
            return fileFinder.Templates;

        }
    }
}
