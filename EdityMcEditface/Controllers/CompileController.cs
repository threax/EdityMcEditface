using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Models.Compiler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]")]
    public class CompileController : Controller
    {
        private SiteBuilder builder;

        public CompileController(SiteBuilder builder)
        {
            this.builder = builder;
        }

        [HttpPost]
        public async Task<CompilerResult> Index()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            await builder.BuildSite();

            return new CompilerResult()
            {
                ElapsedSeconds = sw.Elapsed.TotalSeconds
            };
        }
    }
}
